using MovieRecommender.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Recommending
{
    public class UserWeightModel<TKey, TValue>
    {
        public IDictionary<TKey, TValue> GenreMap { get; set; } = new Dictionary<TKey, TValue>();
        public IDictionary<TKey, TValue> KeyWordMap { get; set; } = new Dictionary<TKey, TValue>();
    }

    public static class UserWeightHelper
    {
        public static UserWeightModel<string, int> CreateWeightModel(IEnumerable<Movie> likedMovies)
        {
            var statModel = new UserWeightModel<string, int>();

            foreach (var movie in likedMovies)
            {
                // fill genre map
                foreach (var genre in movie.Genres)
                {
                    if (!statModel.GenreMap.ContainsKey(genre))
                    {
                        statModel.GenreMap.Add(genre, 1);
                    }
                    else
                    {
                        statModel.GenreMap[genre] += 1;
                    }
                }

                // fill keyword map
                foreach (var keyword in movie.KeyWords)
                {
                    if (!statModel.KeyWordMap.ContainsKey(keyword))
                    {
                        statModel.KeyWordMap.Add(keyword, 1);
                    }
                    else
                    {
                        statModel.KeyWordMap[keyword] += 1;
                    }
                }
            }
            return statModel;
        }
    }
}