using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MovieRecommender.Database.Models;
using MongoDB.Driver;
using MovieRecommender.Models;

namespace MovieRecommender.Database.CollectionAPI
{
    public class MongoUserExperimentRepository : IUserExperimentRepository
    {
        private IMongoCollection<UserExperimentProgress> _collection;

        public MongoUserExperimentRepository(MongoDbConnectionPool dbPool)
        {
            _collection = dbPool.Database.GetCollection<UserExperimentProgress>("user_experiment_progress");
        }

        public IEnumerable<ClickInfo> GetClickProgresssForUser(string userName, Experiment experimentResult)
        {
            var result = new List<ClickInfo>();

            var userNameFilter = Builders<UserExperimentProgress>.Filter.Where(x => x.UserName == userName);
            var progressData = _collection.Find(userNameFilter).ToList().OrderBy(x => x.TimeStamp);

            IList<MovieChoice> watched = experimentResult.WatchedChoice.ToList();
            IList<MovieChoice> wouldWatch = experimentResult.WouldWatchChoice.ToList();
            IList<MovieChoice> wouldNotWatch = experimentResult.WouldNotWatchChoice.ToList();

            foreach (var userProgress in progressData)
            {
                ClickInfo clickInfo = new ClickInfo();

                switch (userProgress.ChoiceValue)
                {
                    case "wouldWatch":
                        if (wouldWatch.FirstOrDefault() != null)
                        {
                            clickInfo.IsSentimentClick = wouldWatch.First().IsSentiment;
                            if (clickInfo.IsSentimentClick == false)
                                clickInfo.IsSentimentClick = false;
                            clickInfo.ClickType = ClickType.WouldWatch;
                            result.Add(clickInfo);
                            wouldWatch.RemoveAt(0);
                        }
                        break;
                    case "wouldNotWatch":
                        if (wouldNotWatch.FirstOrDefault() != null)
                        {
                            clickInfo.IsSentimentClick = wouldNotWatch.First().IsSentiment;
                            clickInfo.ClickType = ClickType.WouldNotWatch;
                            result.Add(clickInfo);
                            wouldNotWatch.RemoveAt(0);
                        }
                        break;
                    case "saw":
                        if (watched.FirstOrDefault() != null)
                        {
                            clickInfo.IsSentimentClick = watched.First().IsSentiment;
                            clickInfo.ClickType = ClickType.Saw;
                            result.Add(clickInfo);
                            watched.RemoveAt(0);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(userProgress.ChoiceValue)); // should never happen
                }
            }

            return result;
        }



        public void LogUserProgress(UserExperimentProgress progress)
        {
            _collection.InsertOneAsync(progress);
        }

    }
}