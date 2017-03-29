using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Models
{
    public class SingleLikeModel
    {
        public bool IsLike { get; set; }
        public string IMDbId { get; set; }
    }

    public class MultiLikeModel
    {
        public bool IsLike { get; set; }
        public IEnumerable<string> IMDbIds { get; set; }
    }

    public class InterestModel
    {
        public bool IsNotInterested { get; set; }
        public string IMDbId { get; set; }
    }
}