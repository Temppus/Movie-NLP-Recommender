using MovieRecommender.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Models
{
    public class MovieDetailModel
    {
        public bool IsLikedMovie { get; set; }
        public int TMDBId { get; set; }
        public string IMDBId { get; set; }
        public string Title { get; set; }
        public string Director { get; set; }
        public int PublicationYear { get; set; }
        public double Rating { get; set; }
        public int RatingCount { get; set; }
        public string Overview { get; set; }
        public string ImageURI { get; set; }
        public string TrailerImageURI { get; set; }
        public string TrailerVideoURI { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
        public List<string> Keywords { get; set; } = new List<string>();
        public List<ActorModel> ActorsInfo { get; set; } = new List<ActorModel>();
        public IEnumerable<MovieSuggestionModel> MovieSuggestions { get; set; } = new List<MovieSuggestionModel>();
        public IEnumerable<ReviewStructure> Reviews { get; set; } = new List<ReviewStructure>();
    }

    public class ActorModel
    {   
        public string ActorName { get; set; }
        public string ImageURI { get; set; }
        public string As { get; set; }
    }
}