using MovieRecommender.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender.Recommending
{
    public interface IRecommender
    {
        IEnumerable<Movie> RecommendForUser(string userName);
    }
}
