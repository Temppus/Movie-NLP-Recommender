using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender.Database.CollectionAPI
{
    public interface IUserRepository
    {
        bool CheckIfLikedMovie(string userName, string imdbId);
        void UserLikedMovie(string userName, string imdbId);
        void UserUnlikedMovie(string userName, string imdbId);
    }
}
