using MongoDB.Driver;
using MovieRecommender.App_Start;
using MovieRecommender.Database;
using MovieRecommender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using MovieRecommender.Database.Models;

namespace MovieRecommender.StartupHooks
{
    public class IntegrityManager : IRunOnApplicationStart
    {
        private IMongoCollection<ApplicationUser> _users;
        private IMongoCollection<Movie> _movies;

        public IntegrityManager()
        {
            var dbPool = UnityConfig.GetConfiguredContainer().Resolve<MongoDbConnectionPool>();
            _users = dbPool.Database.GetCollection<ApplicationUser>("users");
            _movies = dbPool.Database.GetCollection<Movie>("movies");
        }

        public void Start()
        {
            CheckAndFixUpUserCollectionIntegrity();
            UpdatePosters();
        }

        private void CheckAndFixUpUserCollectionIntegrity()
        {
            // remove null values from liked movies, I fucked up once :)
            var updateDefinition = Builders<ApplicationUser>.Update.Pull(u => u.LikedMovies, null);
            var emptyFilter = Builders<ApplicationUser>.Filter.Empty;

            _users.UpdateMany(emptyFilter, updateDefinition);
        }

        private void UpdatePosters()
        {
            var filter1 = Builders<Movie>.Filter.Where(m => m.IMDBId == "tt0378194");
            var update1 = Builders<Movie>.Update.Set(m => m.ImageURI, "https://images-na.ssl-images-amazon.com/images/M/MV5BNjA2MjZiNmQtYmM0My00NmQwLWE2OGMtZGU0MWYzODI5YjY1XkEyXkFqcGdeQXVyNjU0OTQ0OTY@._V1_UY268_CR3,0,182,268_AL_.jpg");

            var filter2 = Builders<Movie>.Filter.Where(m => m.IMDBId == "tt0209144");
            var update2 = Builders<Movie>.Update.Set(m => m.ImageURI, "https://images-na.ssl-images-amazon.com/images/M/MV5BZTcyNjk1MjgtOWI3Mi00YzQwLWI5MTktMzY4ZmI2NDAyNzYzXkEyXkFqcGdeQXVyNjU0OTQ0OTY@._V1_UY268_CR0,0,182,268_AL_.jpg");


            _movies.UpdateOne(filter1, update1);
            _movies.UpdateOne(filter2, update2);
        }
    }
}