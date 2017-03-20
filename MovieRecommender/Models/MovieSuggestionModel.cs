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
        public bool IsLikedByUser { get; set; }
        public string Title { get; set; }
        public int PublicationYear { get; set; }
        public string IMDBId { get; set; }
        public string ImageURI { get; set; }
        public double Rating { get; set; }
        public int RatingCount { get; set; }
        public string Overview { get; set; }
        public string Director { get; set; }
        public IEnumerable<string> Genres { get; set; } = new List<string>();
        public Explanation Explanation { get; set; } = new Explanation();
    }

    public class Explanation
    {
        public bool IsSentimental { get; set; } = false;

        public IList<SentimentHolder> SentimentHolders { get; set; } = new List<SentimentHolder>();

        public Explanation()
        {

        }
    }

    public class SentimentHolder
    {
        /// <summary>
        /// Sentiment sentence
        /// </summary>
        public string Sentence { get; set; }

        /// <summary>
        /// Rounded to interval 0 - 100
        /// </summary>
        public int Score { get; set; }

        public SentimentHolder(string sentence, double score)
        {
            Sentence = sentence;
            Score = (int)(score * 100.0d);
        }
    }
}