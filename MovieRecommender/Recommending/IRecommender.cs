using MovieRecommender.Database.Models;
using MovieRecommender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender.Recommending
{
    public interface IRecommender
    {
        IEnumerable<MovieSuggestionModel> RecommendForUser(string userName);
    }
}
