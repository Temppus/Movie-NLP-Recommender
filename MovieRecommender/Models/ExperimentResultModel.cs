using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Models
{
    public class ExperimentResultModel
    {
        public IEnumerable<string> WatchedIds { get; set; } = new List<string>();
        public IEnumerable<string> NotWatchedIds { get; set; } = new List<string>();
        public IEnumerable<string> WouldWatchIds { get; set; } = new List<string>();
        public IEnumerable<string> WouldNotWatchIds { get; set; } = new List<string>();
    }
}