using System;
using System.Collections.Generic;
using MovieRecommender.Database.Models;
using MovieRecommender.Database.CollectionAPI;
using System.Linq;
using MovieRecommender.Models;

namespace MovieRecommender.Recommending
{
    /*public class NlpRecommender : IRecommender
    {
        private readonly IMovieMentionRepository _movieMentionRepository;
        private readonly IUserRepository _userStore;

        public NlpRecommender(IUserRepository userStore, IMovieMentionRepository movieMentionRepository)
        {
            _movieMentionRepository = movieMentionRepository;
            _userStore = userStore;
        }

        public IEnumerable<MovieSuggestionModel> RecommendForUser(string userName, IEnumerable<string> genres, int fromYear, int toYear, double minRating, int limit)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MovieSuggestionModel> RecommendForUserByMovie(string userName, string movieId)
        {
            throw new NotImplementedException();
        }

        IEnumerable<MovieSuggestionModel> IRecommender.RecommendForUser(string userName)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));

            var recommendedMovies = new List<MovieMention>();

            var likedMovieIds = _userStore.FindLikedMovieIds(userName);
            IEnumerable<MovieMention> movieMantions = _movieMentionRepository.GetMovieMentionsForMovies(likedMovieIds);

            foreach (MovieMention movieMention in movieMantions)
            {
                // If user saw and liked mentioned film -> skip it
                if (likedMovieIds.Contains(movieMention.MentionedIMDBId))
                    continue;

                recommendedMovies.Add(movieMention);
            }

            return new List<MovieSuggestionModel>();
        }
    }*/
}