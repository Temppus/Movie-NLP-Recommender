using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Database.Models
{
    [BsonIgnoreExtraElements]
    public class Movie
    {
        [BsonId]
        public ObjectId _id { get; set; }

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
        public List<string> Genres { get; set; }
        public List<string> KeyWords { get; set; }
        public List<ActorsInfo> ActorsInfo { get; set; }

        public ObjectId ReviewId { get; set; }
    }

    public class ActorsInfo
    {
        public string ActorName { get; set; }
        public string ImageURI { get; set; }
        public string As { get; set; }
    }
}