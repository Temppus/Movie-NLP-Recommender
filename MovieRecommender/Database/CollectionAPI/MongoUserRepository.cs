using MongoDB.Bson;
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

        public bool CheckIfUserLikedMovie(string userName, string imdbId)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));

            if (imdbId == null)
                throw new ArgumentNullException(nameof(imdbId));

            var userNameFilter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);
            var movieIdFilter = Builders<ApplicationUser>.Filter.ElemMatch(u => u.LikedMovies, l => l.IMDBId == imdbId);

            var filter = Builders<ApplicationUser>.Filter.And(userNameFilter, movieIdFilter);

            return _collection.Find(filter).ToList().FirstOrDefault() != null;
        }

        public IEnumerable<string> FindLikedMovieIds(string userName)
        {
            return FindLikedMoviesInfo(userName).Select(m => m.IMDBId);
        }

        public IEnumerable<MovieLikeInfo> FindLikedMoviesInfo(string userName)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));

            var filter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);
            var user = _collection.Find(filter).FirstOrDefault();

            if (user == null)
                return new List<MovieLikeInfo>();

            return user.LikedMovies;
        }

        public IDictionary<string, bool> GetUserLikedMovieMappings(string userName, IEnumerable<string> imdbIds)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));

            if (imdbIds == null)
                throw new ArgumentNullException(nameof(imdbIds));

            var userNameFilter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);
            var idsFilter = Builders<ApplicationUser>.Filter.ElemMatch(u => u.LikedMovies, l => imdbIds.Contains(l.IMDBId));
            var filter = Builders<ApplicationUser>.Filter.And(userNameFilter, idsFilter);

            var likedMoviesInfo = _collection.Find(filter).Project(m => m.LikedMovies).ToList();

            var mapppings = new Dictionary<string, bool>();

            foreach(var imdbId in imdbIds)
            {
                if (likedMoviesInfo.FirstOrDefault(l => l.FirstOrDefault(i => i.IMDBId == imdbId) != null) != null)
                    mapppings.Add(imdbId, true);
                else
                    mapppings.Add(imdbId, false);
            }

            return mapppings;
        }

        public void UserLikedMovie(string userName, string imdbId)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));

            if (imdbId == null)
                throw new ArgumentNullException(nameof(imdbId));

            var updateDefinition = Builders<ApplicationUser>.Update.AddToSet(u => u.LikedMovies, new MovieLikeInfo() { IMDBId = imdbId });
            var filter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);

            _collection.UpdateOne(filter, updateDefinition);
        }

        public void UserUnlikedMovie(string userName, string imdbId)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));

            if (imdbId == null)
                throw new ArgumentNullException(nameof(imdbId));

            var updateDefinition = Builders<ApplicationUser>.Update.PullFilter(u => u.LikedMovies, x => x.IMDBId == imdbId);
            var filter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);

            _collection.UpdateOne(filter, updateDefinition);
        }

        public void AddMovieToNotInterested(string userName, string imdbId)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));

            if (imdbId == null)
                throw new ArgumentNullException(nameof(imdbId));

            var updateDefinition = Builders<ApplicationUser>.Update.AddToSet(u => u.NotInterestedMovies, new MovieLikeInfo() { IMDBId = imdbId });
            var filter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);

            _collection.UpdateOne(filter, updateDefinition);
        }

        public void RemoveMovieFromNotInterested(string userName, string imdbId)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));

            if (imdbId == null)
                throw new ArgumentNullException(nameof(imdbId));

            var updateDefinition = Builders<ApplicationUser>.Update.PullFilter(u => u.NotInterestedMovies, x => x.IMDBId == imdbId);
            var filter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);

            _collection.UpdateOne(filter, updateDefinition);
        }

        public bool CheckIfUserHasMovieInNotInterested(string userName, string imdbId)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));

            if (imdbId == null)
                throw new ArgumentNullException(nameof(imdbId));

            var userNameFilter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);
            var movieIdFilter = Builders<ApplicationUser>.Filter.ElemMatch(u => u.NotInterestedMovies, n => n.IMDBId == imdbId);

            var filter = Builders<ApplicationUser>.Filter.And(userNameFilter, movieIdFilter);

            return _collection.Find(filter).ToList().FirstOrDefault() != null;
        }

        public IEnumerable<string> GetNotInterestedMovieIdsForUser(string userName)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));

            var userNameFilter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);
            var user = _collection.Find(userNameFilter).Single();

            return user.NotInterestedMovies.Select(x => x.IMDBId);
        }
    }
}