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
        IEnumerable<MovieLikeInfo> FindLikedMovies(string userName);
        bool CheckIfUserLikedMovie(string userName, string imdbId);
        IDictionary<string, bool> GetUserLikedMovieMappings(string userName, IEnumerable<string> imdbIds);
        void UserLikedMovie(string userName, string imdbId);
        void UserUnlikedMovie(string userName, string imdbId);
    }
}
