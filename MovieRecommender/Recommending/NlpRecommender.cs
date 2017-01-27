using System;
using System.Collections.Generic;
using MovieRecommender.Database.Models;
using MovieRecommender.Database.CollectionAPI;
using System.Linq;

namespace MovieRecommender.Recommending
{
    public class NlpRecommender : IRecommender
    {
        private readonly IMovieMentionRepository _movieMentionRepository;
        private readonly IUserRepository _userStore;

        public NlpRecommender(IUserRepository userStore, IMovieMentionRepository movieMentionRepository)
        {
            _movieMentionRepository = movieMentionRepository;
            _userStore = userStore;
        }

        public IEnumerable<Movie> RecommendForUser(string userName)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));

            var recommendedMovies = new List<MovieMention>();

            var likedMovies = _userStore.FindLikedMovies(userName).Select(m => m.IMDBId);
            IEnumerable<MovieMention> movieMantions = _movieMentionRepository.GetMovieMentionsForMovies(likedMovies);

            foreach (MovieMention movieMention in movieMantions)
            {
                // If user saw and liked mentioned film -> skip it
                if (likedMovies.Contains(movieMention.MentionedIMDBId))
                    continue;

                recommendedMovies.Add(movieMention);
            }

            return new List<Movie>();
        }
    }
}