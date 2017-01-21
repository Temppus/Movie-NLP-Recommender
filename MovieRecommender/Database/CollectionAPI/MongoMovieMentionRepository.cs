using MongoDB.Driver;
using MovieRecommender.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Database.CollectionAPI
{
    public class MongoMovieMentionRepository : IMovieMentionRepository
    {
        private IMongoCollection<MovieMention> _collection;

        public MongoMovieMentionRepository(MongoDbConnectionPool dbPool)
        {
            _collection = dbPool.Database.GetCollection<MovieMention>("movie_mentions");
        }
    }
}