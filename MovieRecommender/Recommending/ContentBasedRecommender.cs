﻿using System;
using System.Collections.Generic;
using System.Linq;
using MovieRecommender.Database.Models;
using MovieRecommender.Database.CollectionAPI;
using MovieRecommender.Extensions;
using MongoDB.Bson.Serialization;
using MovieRecommender.Models;
using MongoDB.Bson;
using MovieRecommender.Utils;

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

            for (int i = 0; i < 25; i++)
            {
                string keyword = weightModel.KeyWordMap.RandomElementByWeight(g => g.Value).Key;

                weightModel.KeyWordMap.Remove(keyword);

                if (!weightedKeywords.Contains(keyword))
                    weightedKeywords.Add(keyword);
            }

            var exceptMovieIds = likedMovieIds.Concat(_userStore.GetNotInterestedMovieIdsForUser(userName));

            var suggestedMovies = _movieStore.FindSimilarMovies(genres, weightedKeywords, exceptMovieIds,
                                                                limit, fromYear, toYear, 500, minRating);

            return suggestedMovies.Select(s => BsonSerializer.Deserialize<MovieSuggestionModel>(s));
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
    }
}