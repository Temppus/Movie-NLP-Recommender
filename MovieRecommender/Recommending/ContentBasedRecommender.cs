using System;
using System.Collections.Generic;
using System.Linq;
using MovieRecommender.Database.Models;
using MovieRecommender.Database.CollectionAPI;
using MovieRecommender.Extensions;
using MongoDB.Bson.Serialization;
using MovieRecommender.Models;
using MongoDB.Bson;

namespace MovieRecommender.Recommending
{
    public class ContentBasedRecommender : IRecommender
    {
        private readonly IUserRepository _userStore;
        private readonly IMovieRepository _movieStore;

        public ContentBasedRecommender(IUserRepository userStore, IMovieRepository movieStore)
        {
            _userStore = userStore;
            _movieStore = movieStore;
        }

        public IEnumerable<MovieSuggestionModel> RecommendForUser(string userName)
        {
            var likedMovieInfos = _userStore.FindLikedMovies(userName);
            var likedMovies = _movieStore.FindMoviesByIMDbIds(likedMovieInfos.Select(m => m.IMDBId));

            var weightModel = CreateWeightModel(likedMovies);

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

            var exceptMovieIds = likedMovieInfos.Select(l => l.IMDBId).Concat(_userStore.GetNotInterestedMovieIdsForUser(userName));

            var suggestedMovies = _movieStore.FindSimilarMovies(weightedGenres, weightedKeywords, exceptMovieIds,
                                                                20, 1990, 500, 5.0d);

            return suggestedMovies.Select(s => BsonSerializer.Deserialize<MovieSuggestionModel>(s));
        }

        private UserWeightModel<string, int> CreateWeightModel(IEnumerable<Movie> likedMovies)
        {
            var statModel = new UserWeightModel<string, int>();

            foreach (var movie in likedMovies)
            {
                // fill genre map
                foreach (var genre in movie.Genres)
                {
                    if (!statModel.GenreMap.ContainsKey(genre))
                    {
                        statModel.GenreMap.Add(genre, 1);
                    }
                    else
                    {
                        statModel.GenreMap[genre] += 1;
                    }
                }

                // fill keyword map
                foreach (var keyword in movie.Keywords)
                {
                    if (!statModel.KeyWordMap.ContainsKey(keyword))
                    {
                        statModel.KeyWordMap.Add(keyword, 1);
                    }
                    else
                    {
                        statModel.KeyWordMap[keyword] += 1;
                    }
                }
            }
            return statModel;
        }
    }

    public class UserWeightModel<TKey, TValue>
    {
        public IDictionary<TKey, TValue> GenreMap { get; set; } = new Dictionary<TKey, TValue>();
        public IDictionary<TKey, TValue> KeyWordMap { get; set; } = new Dictionary<TKey, TValue>();
    }
}