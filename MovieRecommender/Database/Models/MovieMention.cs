using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Database.Models
{
    public class MovieMention
    {
        [BsonId]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();
        public string FromIMDBId { get; set; }
        public string MentionedIMDBId { get; set; }
        public string MovieName { get; set; }
        public string ReviewTitle { get; set; }
        public string ConceptName { get; set; }
        public List<SentimentSentence> PositiveSentences { get; set; } = new List<SentimentSentence>();
    }

    public class SentimentSentence
    {
        public string Text { get; set; }
        public float Score { get; set; }
        public float Magnitude { get; set; }
        public string ContextBefore { get; set; }
        public string ContextAfter { get; set; }
    }
}