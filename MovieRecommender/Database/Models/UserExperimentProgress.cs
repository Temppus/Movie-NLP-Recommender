using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Database.Models
{
    [BsonIgnoreExtraElements]
    public class UserExperimentProgress
    {
        public string UserName { get; set; }
        public string ChoiceValue { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}