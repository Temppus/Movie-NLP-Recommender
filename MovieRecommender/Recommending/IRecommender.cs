using MovieRecommender.Database.Models;
using MovieRecommender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender.Recommending
{
    public interface IRecommender : IExplainable
    {
        IEnumerable<MovieSuggestionModel> RecommendForUser(string userName);
        IEnumerable<MovieSuggestionModel> RecommendForUserByMovie(string userName, string movieId);
        IEnumerable<MovieSuggestionModel> RecommendForUser(string userName, IEnumerable<string> genres, int fromYear, int toYear, double minRating, int limit);
        IEnumerable<MovieSuggestionModel> RecommendForUser(string userName, IEnumerable<string> genres, int fromYear, int toYear, double minRating, int limit, IEnumerable<string> exceptIds);
    }

}
