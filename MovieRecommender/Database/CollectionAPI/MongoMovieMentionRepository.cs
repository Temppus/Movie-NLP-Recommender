using MongoDB.Driver;
using MovieRecommender.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MovieRecommender.Extensions;

namespace MovieRecommender.Database.CollectionAPI
{
    public class MongoMovieMentionRepository : IMovieMentionRepository
    {
        private IMongoCollection<MovieMention> _collection;

        public MongoMovieMentionRepository(MongoDbConnectionPool dbPool)
        {
            _collection = dbPool.Database.GetCollection<MovieMention>("movie_mentions");
        }

        public IEnumerable<MovieMention> GetMovieMentionsForMovies(IEnumerable<string> movieIds)
        {
            movieIds.ThrowIfNull(nameof(movieIds));

            IList<MovieMention> mentions = new List<MovieMention>();

            var filter = Builders<MovieMention>.Filter.In(mm => mm.FromIMDBId, movieIds);
            var likedMoviesInfo = _collection.Find(filter).ToList();

            return likedMoviesInfo;
        }
    }
}