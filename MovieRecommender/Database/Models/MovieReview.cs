using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Database.Models
{
    [BsonIgnoreExtraElements]
    public class MovieReview
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public bool SentimentExtracted { get; set; }
        public string ImdbId { get; set; }
        public List<ReviewStructure> Reviews { get; set; } = new List<ReviewStructure>();
    }

    [BsonIgnoreExtraElements]
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

        public float SentimentScore { get; set; }
        public float SentimentMagnitude { get; set; }

        public int GetUsefullnessVotes()
        {
            int score = 0;

            //9 out of 9 people found the following review useful:
            if (int.TryParse(UsefullnessDescription.Split(new string[] { " out of " }, StringSplitOptions.None).FirstOrDefault(), out score))
                return score;

            return score;
        }
    }
}