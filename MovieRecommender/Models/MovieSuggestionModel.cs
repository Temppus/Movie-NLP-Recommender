using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Models
{
    [BsonIgnoreExtraElements]
    public class MovieSuggestionModel
    {
        public string Title { get; set; }
        public string IMDBId { get; set; }
        public string ImageURI { get; set; }
        public double Rating { get; set; }
        public int RatingCount { get; set; }
        public string Overview { get; set; }
        public string Director { get; set; }
        public IEnumerable<string> Genres { get; set; } = new List<string>();
    }
}