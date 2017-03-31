using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Models
{
    public class ExperimentResultModel
    {
        public IEnumerable<MovieChoice> WatchedChoice { get; set; } = new List<MovieChoice>();
        public IEnumerable<MovieChoice> WouldWatchChoice { get; set; } = new List<MovieChoice>();
        public IEnumerable<MovieChoice> WouldNotWatchChoice { get; set; } = new List<MovieChoice>();

        public IEnumerable<string> WatchedIds()
        {
            return WatchedChoice.Select(x => x.IMDBID);
        }

        public IEnumerable<string> WouldWatchIds()
        {
            return WouldWatchChoice.Select(x => x.IMDBID);
        }

        public IEnumerable<string> WouldNotWatchIds()
        {
            return WouldNotWatchChoice.Select(x => x.IMDBID);
        }
    }

    public class MovieChoice
    {
        public string IMDBID { get; set; }
        public bool IsSentiment { get; set; }
    }
}