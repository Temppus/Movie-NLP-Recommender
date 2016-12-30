using MovieRecommender.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Models
{
    public class QuickSearchResponseItem
    {
        public QuickSearchResponseItem(Movie movie)
        {
            title = movie.Title;
            url = $"Movie/Details/?imdbId={movie.IMDBId}";

            string director = string.IsNullOrEmpty(movie.Director) ? "unknown" : movie.Director;

            description = $"Director: {director}. Rating: {movie.Rating}";
            image = $"{movie.ImageURI}";
        }

        /// <summary>
        /// Property name starting with lowercase to conform sematic ui expected response
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// Property name starting with lowercase to conform sematic ui expected response
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// Property name starting with lowercase to conform sematic ui expected response
        /// </summary>
        public string image { get; set; }

        /// <summary>
        /// Property name starting with lowercase to conform sematic ui expected response
        /// </summary>
        public string description { get; set; }
    }

    public class QuickSearchResponse
    {
        public QuickSearchResponse(IEnumerable<Movie> movie)
        {
            results = movie.Select(m => new QuickSearchResponseItem(m));
        }

        /// <summary>
        /// Property name starting with lowercase to conform sematic ui expected response
        /// </summary>
        public IEnumerable<QuickSearchResponseItem> results { get; set; }
    }
}