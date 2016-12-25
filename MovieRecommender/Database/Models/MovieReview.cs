using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Database.Models
{
    public class MovieReview
    {
        [BsonId]
        public ObjectId _id { get; set; }

        public string ImdbId { get; set; }

        public List<ReviewStructure> Reviews { get; set; }
    }

    public class ReviewStructure
    {
        public string UsefullnessDescription { get; set; }
        public string Title { get; set; }
        public string UserURI { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public string UserCountry { get; set; }
        public string ReviewDate { get; set; }
        public bool IsSpoilerReview { get; set; }
        public int Rating { get; set; }
    }
}