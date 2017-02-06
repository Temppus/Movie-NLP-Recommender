using MovieRecommender.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Models
{
    public class ColdStartModel
    {
        public IDictionary<string, IEnumerable<Movie>> MoviesDic { get; set; } = new Dictionary<string, IEnumerable<Movie>>();
    }
}