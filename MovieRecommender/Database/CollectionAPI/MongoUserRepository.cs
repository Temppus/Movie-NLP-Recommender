using MongoDB.Driver;
using MovieRecommender.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieRecommender.Database.CollectionAPI
{
    public class MongoUserRepository : IUserRepository
    {
        private IMongoCollection<ApplicationUser> _collection;

        public MongoUserRepository(MongoDbConnectionPool dbPool)
        {
            _collection = dbPool.Database.GetCollection<ApplicationUser>("users");
        }

        public bool CheckIfLikedMovie(string userName, string imdbId)
        {
            if (userName == null)
                throw new ArgumentNullException(userName);

            if (imdbId == null)
                throw new ArgumentNullException(imdbId);

            var filter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName && u.LikedMovies.Contains(imdbId));
            return _collection.Find(filter).ToList().FirstOrDefault() != null;
        }

        public void UserLikedMovie(string userName, string imdbId)
        {
            if (userName == null)
                throw new ArgumentNullException(userName);

            if (imdbId == null)
                throw new ArgumentNullException(imdbId);

            var updateDefinition = Builders<ApplicationUser>.Update.AddToSet(u => u.LikedMovies, imdbId);
            var filter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);

            _collection.UpdateOne(filter, updateDefinition);
        }

        public void UserUnlikedMovie(string userName, string imdbId)
        {
            if (userName == null)
                throw new ArgumentNullException(userName);

            if (imdbId == null)
                throw new ArgumentNullException(imdbId);

            var updateDefinition = Builders<ApplicationUser>.Update.Pull(u => u.LikedMovies, imdbId);
            var filter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);

            _collection.UpdateOne(filter, updateDefinition);
        }
    }
}