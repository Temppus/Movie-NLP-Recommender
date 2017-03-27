using System;
using System.Collections.Generic;
using System.Linq;
using MovieRecommender.Database.Models;
using MovieRecommender.Database.CollectionAPI;
using MovieRecommender.Extensions;
using MongoDB.Bson.Serialization;
using MovieRecommender.Models;
using MongoDB.Bson;
using MovieRecommender.Utils;
using MovieRecommender.App_Start;
using Microsoft.Practices.Unity;

namespace MovieRecommender.Recommending
{
    public class ContentBasedRecommender : IRecommender
    {
        private readonly IUserRepository _userStore;
        private readonly IMovieRepository _movieStore;

        private static double _minRating = 5.5d;
        private static int _minYear = 1990;

        public ContentBasedRecommender(IUserRepository userStore, IMovieRepository movieStore)
        {
            _userStore = userStore;
            _movieStore = movieStore;
        }

        public IEnumerable<MovieSuggestionModel> RecommendForUser(string userName)
        {
            var likedMovieIds = _userStore.FindLikedMovieIds(userName);
            var likedMovies = _movieStore.FindMoviesByIMDbIds(likedMovieIds);

            var weightModel = UserWeightHelper.CreateWeightModel(likedMovies);

            IList<string> weightedGenres = new List<string>();

            for (int i = 0; i < 10; i++)
            {
                string genre = weightModel.GenreMap.RandomElementByWeight(g => g.Value).Key;

                weightModel.GenreMap.Remove(genre);

                if (!weightedGenres.Contains(genre))
                    weightedGenres.Add(genre);
            }

            IList<string> weightedKeywords = new List<string>();

            for (int i = 0; i < 25; i++)
            {
                string keyword = weightModel.KeyWordMap.RandomElementByWeight(g => g.Value).Key;

                weightModel.KeyWordMap.Remove(keyword);

                if (!weightedKeywords.Contains(keyword))
                    weightedKeywords.Add(keyword);
            }

            var exceptMovieIds = likedMovieIds.Concat(_userStore.GetNotInterestedMovieIdsForUser(userName));

            var suggestedMovies = _movieStore.FindSimilarMovies(weightedGenres, weightedKeywords, exceptMovieIds,
                                                                20, _minYear, int.MaxValue, 500, _minRating);

            return suggestedMovies.Select(s => BsonSerializer.Deserialize<MovieSuggestionModel>(s));
        }

        public IEnumerable<MovieSuggestionModel> RecommendForUser(string userName, IEnumerable<string> genres, int fromYear, int toYear, double minRating, int limit)
        {
            var likedMovieIds = _userStore.FindLikedMovieIds(userName);
            var likedMovies = _movieStore.FindMoviesByIMDbIds(likedMovieIds);

            var weightModel = UserWeightHelper.CreateWeightModel(likedMovies);

            IList<string> weightedKeywords = new List<string>();
            IDictionary<string, int> KeywordsDicCopy = new Dictionary<string, int>(weightModel.KeyWordMap);

            for (int i = 0; i < 25; i++)
            {
                string keyword = weightModel.KeyWordMap.RandomElementByWeight(g => g.Value).Key;

                weightModel.KeyWordMap.Remove(keyword);

                if (!weightedKeywords.Contains(keyword))
                    weightedKeywords.Add(keyword);
            }

            var exceptMovieIds = likedMovieIds.Concat(_userStore.GetNotInterestedMovieIdsForUser(userName));

            var suggestedMovies = _movieStore.FindSimilarMovies(genres, weightedKeywords, exceptMovieIds,
                                                                limit, fromYear, toYear, 50000, minRating);

            var suggestions = suggestedMovies.Select(s => BsonSerializer.Deserialize<MovieSuggestionModel>(s)).ToList();

            // Explain by random method
            Random rnd = new Random();

            var sentimentExplanations = ExplainBySentiment(suggestions.Select(m => m.IMDBId));
            var keywordExplanations = ExplainByKeywords(suggestions.Select(m => m.IMDBId), KeywordsDicCopy);

            var randomExplanations = new List<ExplanationTuple>();

            bool explainBySentiment = rnd.Next(0, 2) == 1;

            for (int i = 0; i < suggestions.Count(); i++)
            {
                var suggestion = suggestions.ElementAt(i);

                if (explainBySentiment)
                {
                    randomExplanations.Add(sentimentExplanations.First(s => s.ImdbId == suggestion.IMDBId));
                }
                else
                {
                    randomExplanations.Add(keywordExplanations.First(s => s.ImdbId == suggestion.IMDBId));
                }

                explainBySentiment = !explainBySentiment;
            }

            foreach (var explTuple in randomExplanations)
            {
                var suggestion = suggestions.FirstOrDefault(m => m.IMDBId == explTuple.ImdbId);

                if (suggestion != null)
                    suggestion.Explanation = explTuple.Explanation;
            }

            return suggestions;
        }

        public IEnumerable<MovieSuggestionModel> RecommendForUserByMovie(string userName, string movieId)
        {
            var likedMovieIds = _userStore.FindLikedMovieIds(userName);
            Movie movie = _movieStore.FindMovieByImdbId(movieId);

            var exceptMovieIds = likedMovieIds
                                .Concat(_userStore.GetNotInterestedMovieIdsForUser(userName))
                                .Concat(new List<string>() { movieId });

            var suggestedBsonMovies = _movieStore.FindSimilarMovies(movie.Genres, movie.KeyWords, exceptMovieIds,
                                                                100, _minYear, int.MaxValue, 500, _minRating);

            var suggestedMovies = suggestedBsonMovies.Select(s => BsonSerializer.Deserialize<MovieSuggestionModel>(s));

            SortedList<double, MovieSuggestionModel> priorityList = new SortedList<double, MovieSuggestionModel>(new DuplicateKeyComparer<double>());

            double minYearDiff = suggestedMovies.Select(d => Math.Abs(d.PublicationYear - movie.PublicationYear)).Min();
            double maxYearDiff = suggestedMovies.Select(d => Math.Abs(d.PublicationYear - movie.PublicationYear)).Max();

            double minRatingDiff = suggestedMovies.Select(d => Math.Abs(d.Rating - movie.Rating)).Min();
            double maxRatingDiff = suggestedMovies.Select(d => Math.Abs(d.Rating - movie.Rating)).Max();

            foreach (var suggestedMovie in suggestedMovies)
            {
                int yearDiff = Math.Abs(suggestedMovie.PublicationYear - movie.PublicationYear);
                double ratingDiff = Math.Abs(suggestedMovie.Rating - movie.Rating);

                double normalizedYearDiff = MathUtil.Normalize(yearDiff, minYearDiff, maxYearDiff);
                double normalizedRatingDiff = MathUtil.Normalize(ratingDiff, minRatingDiff, maxRatingDiff);

                priorityList.Add(normalizedYearDiff + normalizedRatingDiff, suggestedMovie);
            }

            return priorityList.Select(x => x.Value);
        }

        public IEnumerable<ExplanationTuple> ExplainBySentiment(IEnumerable<string> imdbIds)
        {
            IList<ExplanationTuple> explanations = new List<ExplanationTuple>();

            var movies = _movieStore.FindMoviesByIMDbIds(imdbIds);
            var reviewStore = UnityConfig.GetConfiguredContainer().Resolve<IReviewRepository>();

            foreach (var movie in movies)
            {
                var reviews = reviewStore.FindReviewsByReviewId(movie.ReviewId).OrderByDescending(r => r.Rating).Take(5);

                var explanation = new Explanation() { IsSentimental = true };

                foreach (var review in reviews)
                {
                    explanation.ExplanationHolders.Add(new ExplanationHolder(sentence: review.Title, score: review.GetUsefullnessVotes()));
                }

                explanations.Add(new ExplanationTuple(movie.IMDBId, explanation));
            }

            return explanations;
        }

        private IEnumerable<ExplanationTuple> ExplainByKeywords(IEnumerable<string> imdbIds, IDictionary<string, int> keywordsDic)
        {
            IList<ExplanationTuple> explanations = new List<ExplanationTuple>();

            var movies = _movieStore.FindMoviesByIMDbIds(imdbIds);

            foreach (var movie in movies)
            {
                var explanation = new Explanation();

                foreach (KeyValuePair<string, int> keyItem in keywordsDic.OrderByDescending(key => key.Value))
                {
                    if (movie.KeyWords.Contains(keyItem.Key))
                        explanation.ExplanationHolders.Add(new ExplanationHolder(sentence: keyItem.Key, score: Math.Log(keyItem.Value)));
                }

                int toAddCount = 5 - explanation.ExplanationHolders.Count;

                if (toAddCount > 0)
                {
                    foreach (var keyword in movie.KeyWords)
                    {
                        var sentimentHolder = explanation.ExplanationHolders.FirstOrDefault(s => s.Sentence == keyword);

                        if (sentimentHolder != null)
                            continue;

                        if (movie.KeyWords.Contains(keyword) && toAddCount-- > 0)
                            explanation.ExplanationHolders.Add(new ExplanationHolder(sentence: keyword, score: explanation.ExplanationHolders.Min(x => x.Score)));
                    }
                }

                explanations.Add(new ExplanationTuple(movie.IMDBId, explanation));
            }

            return explanations;
        }
    }
}