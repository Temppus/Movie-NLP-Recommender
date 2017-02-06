using MovieRecommender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender.Database.CollectionAPI
{
    public interface IUserRepository
    {
        IEnumerable<MovieLikeInfo> FindLikedMoviesInfo(string userName);
        IEnumerable<string> FindLikedMovieIds(string userName);
        bool CheckIfUserLikedMovie(string userName, string imdbId);
        IDictionary<string, bool> GetUserLikedMovieMappings(string userName, IEnumerable<string> imdbIds);
        void UserLikedMovie(string userName, string imdbId);
        void UserUnlikedMovie(string userName, string imdbId);

        IEnumerable<string> GetNotInterestedMovieIdsForUser(string userName);
        bool CheckIfUserHasMovieInNotInterested(string userName, string imdbId);
        void AddMovieToNotInterested(string userName, string imdbId);
        void RemoveMovieFromNotInterested(string userName, string imdbId);
    }
}
