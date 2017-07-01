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
        public ObjectId ReviewId { get; set; }
        public IEnumerable<string> Genres { get; set; } = new List<string>();
        public Explanation Explanation { get; set; } = new Explanation();
    }

    public class Explanation
    {
        public bool IsSentimental { get; set; } = false;

        public IList<ExplanationHolder> ExplanationHolders { get; set; } = new List<ExplanationHolder>();

        public Explanation()
        {

        }
    }

    public class ExplanationHolder
    {
        /// <summary>
        /// Sentiment sentence
        /// </summary>
        public string Sentence { get; set; }

        public double Score { get; set; }

        public ExplanationHolder(string sentence, double score)
        {
            Sentence = sentence;
            Score = score;
        }
    }
}