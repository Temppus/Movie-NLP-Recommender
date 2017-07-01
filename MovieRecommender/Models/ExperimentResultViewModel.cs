using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Models
{
    public class ExperimentResultViewModel
    {
        public IList<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        public List<List<ClickInfo>> UsersClicksByTime { get; set; } = new List<List<ClickInfo>>();

        public IList<int> SentimentWouldWatchList { get; set; } = new List<int>();
        public IList<int> KeyWordsWouldWatchList { get; set; } = new List<int>();

        public IList<double> SawPrecisionAt { get; set; } = new List<double>();
        public IList<double> SentimentPrecisionAt { get; set; } = new List<double>();
        public IList<double> KeywordPrecisionAt { get; set; } = new List<double>();

        public double SawNDCG { get; set; }

        public double SentimentSingleNDCG { get; set; }
        public double KeywordSingleNDCG { get; set; }

        public double SentimentNDCGAll { get; set; }
        public double KeywordNDCGAll { get; set; }

        public ExperimentResultViewModel()
        {
        }
    }

    public class ClickInfo
    {
        public bool IsSentimentClick { get; set; }
        public ClickType ClickType { get; set; }
    }

    public enum ClickType
    {
        WouldWatch,
        WouldNotWatch,
        Saw
    }

}