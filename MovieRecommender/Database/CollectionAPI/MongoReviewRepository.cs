using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MovieRecommender.Database.Models;
using MongoDB.Driver;

namespace MovieRecommender.Database.CollectionAPI
{
    public class MongoReviewRepository : IReviewRepository
    {
        private IMongoCollection<MovieReview> _collection;

        public MongoReviewRepository(MongoDbConnectionPool dbPool)
        {
            _collection = dbPool.Database.GetCollection<MovieReview>("reviews");
        }

        public IEnumerable<ReviewStructure> FindReviewsByReviewId(ObjectId reviewId)
        {
            var review = _collection.Find(r => r._id == reviewId).FirstOrDefault();

            if (review == null)
                return new List<ReviewStructure>();

            return review.Reviews == null ? new List<ReviewStructure>() : review.Reviews;
        }
    }
}