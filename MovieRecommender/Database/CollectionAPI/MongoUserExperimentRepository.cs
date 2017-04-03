using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MovieRecommender.Database.Models;
using MongoDB.Driver;

namespace MovieRecommender.Database.CollectionAPI
{
    public class MongoUserExperimentRepository : IUserExperimentRepository
    {
        private IMongoCollection<UserExperimentProgress> _collection;

        public MongoUserExperimentRepository(MongoDbConnectionPool dbPool)
        {
            _collection = dbPool.Database.GetCollection<UserExperimentProgress>("user_experiment_progress");
        }
        public void LogUserProgress(UserExperimentProgress progress)
        {
            _collection.InsertOneAsync(progress);
        }
    }
}