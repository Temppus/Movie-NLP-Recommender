using MongoDB.Driver;
using MovieRecommender.App_Start;
using MovieRecommender.Database;
using MovieRecommender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;

namespace MovieRecommender.StartupHooks
{
    public class IntegrityManager : IRunOnApplicationStart
    {
        private IMongoCollection<ApplicationUser> _users;

        public IntegrityManager()
        {
            var dbPool = UnityConfig.GetConfiguredContainer().Resolve<MongoDbConnectionPool>();
            _users = dbPool.Database.GetCollection<ApplicationUser>("users");
        }

        private void CheckAndFixUpUserCollectionIntegrity()
        {
            // remove null values from liked movies, I fucked up once :)
            var updateDefinition = Builders<ApplicationUser>.Update.Pull(u => u.LikedMovies, null);
            var emptyFilter = Builders<ApplicationUser>.Filter.Empty;

            _users.UpdateMany(emptyFilter, updateDefinition);
        }

        public void Start()
        {
            CheckAndFixUpUserCollectionIntegrity();
        }
    }
}