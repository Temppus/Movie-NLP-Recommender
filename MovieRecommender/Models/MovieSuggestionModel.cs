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
        public int PublicationYear { get; set; }
        public string IMDBId { get; set; }
        public string ImageURI { get; set; }
        public double Rating { get; set; }
        public int RatingCount { get; set; }
        public string Overview { get; set; }
        public string Director { get; set; }
        public IEnumerable<string> Genres { get; set; } = new List<string>();
        public Explanation Explanation { get; set; } = new Explanation
            (
                new List<SentimentHolder>()
                {
                    new SentimentHolder("Insanely Brilliant ! Nolan has outdone himself !!", 1.0),
                    new SentimentHolder("Nolan's first true masterpiece", 0.98),
                    new SentimentHolder("In a Decade, \"Inception\" May Be A Religion", 0.9),
                    new SentimentHolder("Sci-fi perfection. A truly mesmerizing film", 0.8),
                    new SentimentHolder("The perfect summer blockbuster?", 0.8)
                }
            );
    }

    public class Explanation
    {
        public IEnumerable<SentimentHolder> SentimentHolders { get; set; }

        public Explanation(IEnumerable<SentimentHolder> sentimentHolders)
        {
            SentimentHolders = sentimentHolders;
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