using MongoDB.Bson;
using MovieRecommender.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender.Database.CollectionAPI
{
    public interface IReviewRepository
    {
        IEnumerable<ReviewStructure> FindReviewsByReviewId(ObjectId id);
    }
}
