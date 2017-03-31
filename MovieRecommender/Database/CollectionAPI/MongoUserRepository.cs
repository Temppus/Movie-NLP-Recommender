using MongoDB.Bson;
using MongoDB.Driver;
using MovieRecommender.Exceptions;
using MovieRecommender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MovieRecommender.Extensions;

namespace MovieRecommender.Database.CollectionAPI
{
    public class MongoUserRepository : IUserRepository
    {
        private IMongoCollection<ApplicationUser> _collection;

        public MongoUserRepository(MongoDbConnectionPool dbPool)
        {
            _collection = dbPool.Database.GetCollection<ApplicationUser>("users");
        }

        public void SetOrAddExperiment(string userName, ExperimentResultModel experimentModel)
        {
            userName.ThrowIfNull(nameof(userName));

            var userFilter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);
            var existsFilter = Builders<ApplicationUser>.Filter.Exists(u => u.ExperimentResult, true);
            var notNullFilter = Builders<ApplicationUser>.Filter.Where(u => u.ExperimentResult != null);

            bool exists = _collection.Find(Builders<ApplicationUser>.Filter.And(userFilter, existsFilter, notNullFilter)).ToList().Count > 0;

            if (exists)
            {
                var up1 = Builders<ApplicationUser>.Update.AddToSetEach(u => u.ExperimentResult.WatchedChoice, experimentModel.WatchedChoice);
                var up2 = Builders<ApplicationUser>.Update.AddToSetEach(u => u.ExperimentResult.WouldWatchChoice, experimentModel.WouldWatchChoice);
                var up3 = Builders<ApplicationUser>.Update.AddToSetEach(u => u.ExperimentResult.WouldNotWatchChoice, experimentModel.WouldNotWatchChoice);
                var combineUpdate = Builders<ApplicationUser>.Update.Combine(up1, up2, up3);
                _collection.UpdateOne(userFilter, combineUpdate);
            }
            else
            {
                Experiment experiment = new Experiment()
                {
                    WatchedChoice = experimentModel.WatchedChoice,
                    WouldWatchChoice = experimentModel.WouldWatchChoice,
                    WouldNotWatchChoice = experimentModel.WouldNotWatchChoice,
                };

                var updateDefinition = Builders<ApplicationUser>.Update.Set(u => u.ExperimentResult, experiment);
                _collection.UpdateOne(userFilter, updateDefinition);
            }
        }

        public bool IsExperimentDone(string userName)
        {
            userName.ThrowIfNull(nameof(userName));

            var filter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName && u.ExperimentResult != null);
            var expData = _collection.Find(filter).FirstOrDefault()?.ExperimentResult;

            if (expData == null)
                return false;

            return expData.WouldWatchIds().Count() + expData.WouldNotWatchIds().Count() >= 15;
        }

        public Experiment GetExperimentDataForUser(string userName)
        {
            userName.ThrowIfNull(nameof(userName));

            var userFilter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);
            var existsFilter = Builders<ApplicationUser>.Filter.Exists(u => u.ExperimentResult, true);
            var notNullFilter = Builders<ApplicationUser>.Filter.Where(u => u.ExperimentResult != null);
            var andFilter = Builders<ApplicationUser>.Filter.And(userFilter, existsFilter, notNullFilter);

            return _collection.Find(andFilter).FirstOrDefault()?.ExperimentResult;
        }

        public bool CheckIfUserLikedMovie(string userName, string imdbId)
        {
            userName.ThrowIfNull(nameof(userName));
            imdbId.ThrowIfNull(nameof(imdbId));

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
            userName.ThrowIfNull(nameof(userName));

            var filter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);
            var user = _collection.Find(filter).FirstOrDefault();

            if (user == null)
                return new List<MovieLikeInfo>();

            return user.LikedMovies;
        }

        public IDictionary<string, bool> GetUserLikedMovieMappings(string userName, IEnumerable<string> imdbIds)
        {
            userName.ThrowIfNull(nameof(userName));
            imdbIds.ThrowIfNull(nameof(imdbIds));

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
            userName.ThrowIfNull(nameof(userName));
            imdbId.ThrowIfNull(nameof(imdbId));

            if (CheckIfUserLikedMovie(userName, imdbId))
                throw new UserPreferenceException($"Can not like movie {imdbId} for user {userName}. User already liked movie.");

            var updateDefinition = Builders<ApplicationUser>.Update.AddToSet(u => u.LikedMovies, new MovieLikeInfo() { IMDBId = imdbId });
            var filter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);

            _collection.UpdateOne(filter, updateDefinition);
        }

        public void UserLikedMovies(string userName, IEnumerable<string> imdbIds)
        {
            userName.ThrowIfNull(nameof(userName));
            imdbIds.ThrowIfNull(nameof(imdbIds));

            /*foreach(var imdbId in imdbIds)
            {
                if (CheckIfUserLikedMovie(userName, imdbId))
                    throw new UserPreferenceException($"Can not like movie {imdbId} for user {userName}. User already liked movie.");
            }*/

            var movieLikeInfos = imdbIds.Select(imdbId => new MovieLikeInfo() { IMDBId = imdbId });

            var updateDefinition = Builders<ApplicationUser>.Update.AddToSetEach(u => u.LikedMovies, movieLikeInfos);
            var filter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);

            _collection.UpdateOne(filter, updateDefinition);
        }

        public void UserUnlikedMovie(string userName, string imdbId)
        {
            userName.ThrowIfNull(nameof(userName));
            imdbId.ThrowIfNull(nameof(imdbId));

            if (!CheckIfUserLikedMovie(userName, imdbId))
                throw new UserPreferenceException($"Can not unlike movie {imdbId} for user {userName}. User did not liked this movie.");

            var updateDefinition = Builders<ApplicationUser>.Update.PullFilter(u => u.LikedMovies, x => x.IMDBId == imdbId);
            var filter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);

            _collection.UpdateOne(filter, updateDefinition);
        }

        public void UserUnlikedMovies(string userName, IEnumerable<string> imdbIds)
        {
            userName.ThrowIfNull(nameof(userName));
            imdbIds.ThrowIfNull(nameof(imdbIds));

            foreach (var imdbId in imdbIds)
            {
                if (!CheckIfUserLikedMovie(userName, imdbId))
                    throw new UserPreferenceException($"Can not unlike movie {imdbId} for user {userName}. User did not liked this movie.");
            }

            var movieLikeInfos = imdbIds.Select(imdbId => new MovieLikeInfo() { IMDBId = imdbId });

            var updateDefinition = Builders<ApplicationUser>.Update.PullAll(u => u.LikedMovies, movieLikeInfos);
            var filter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);

            _collection.UpdateOne(filter, updateDefinition);
        }

        public void AddMovieToNotInterested(string userName, string imdbId)
        {
            userName.ThrowIfNull(nameof(userName));
            imdbId.ThrowIfNull(nameof(imdbId));

            if (CheckIfUserHasMovieInNotInterested(userName, imdbId))
                throw new UserPreferenceException($"Can not add movie {imdbId} to not interested set for user {userName}. User already has this movie in not interested.");
            
            var updateDefinition = Builders<ApplicationUser>.Update.AddToSet(u => u.NotInterestedMovies, new MovieLikeInfo() { IMDBId = imdbId });
            var filter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);

            _collection.UpdateOne(filter, updateDefinition);
        }

        public void RemoveMovieFromNotInterested(string userName, string imdbId)
        {
            userName.ThrowIfNull(nameof(userName));
            imdbId.ThrowIfNull(nameof(imdbId));

            if (!CheckIfUserHasMovieInNotInterested(userName, imdbId))
                throw new UserPreferenceException($"Can not remove movie {imdbId} from not interested set for user {userName}. User does not have this movie in not interested.");

            var updateDefinition = Builders<ApplicationUser>.Update.PullFilter(u => u.NotInterestedMovies, x => x.IMDBId == imdbId);
            var filter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);

            _collection.UpdateOne(filter, updateDefinition);
        }

        public bool CheckIfUserHasMovieInNotInterested(string userName, string imdbId)
        {
            userName.ThrowIfNull(nameof(userName));
            imdbId.ThrowIfNull(nameof(imdbId));

            var userNameFilter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);
            var movieIdFilter = Builders<ApplicationUser>.Filter.ElemMatch(u => u.NotInterestedMovies, n => n.IMDBId == imdbId);

            var filter = Builders<ApplicationUser>.Filter.And(userNameFilter, movieIdFilter);

            return _collection.Find(filter).ToList().FirstOrDefault() != null;
        }

        public IEnumerable<string> GetNotInterestedMovieIdsForUser(string userName)
        {
            userName.ThrowIfNull(nameof(userName));

            var userNameFilter = Builders<ApplicationUser>.Filter.Where(u => u.UserName == userName);
            var user = _collection.Find(userNameFilter).Single();

            return user.NotInterestedMovies.Select(x => x.IMDBId);
        }
    }
}