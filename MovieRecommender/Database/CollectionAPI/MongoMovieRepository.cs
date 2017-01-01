﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Driver;
using MovieRecommender.Database.Models;
using System.Runtime.Caching;
using MongoDB.Bson;

namespace MovieRecommender.Database.CollectionAPI
{
    public class MongoMovieRepository : IMovieRepository
    {
        private IMongoCollection<Movie> _collection;

        public MongoMovieRepository(MongoDbConnectionPool dbPool)
        {
            _collection = dbPool.Database.GetCollection<Movie>("movies");
        }

        public IEnumerable<Movie> FilterMovies(int fromYear, int toYear, IEnumerable<string> genres, bool orderDescByRatingCount, int limit, int paginationIndex)
        {
            if (fromYear <= 0)
                throw new ArgumentException(nameof(fromYear) + " must be > 0");

            if (toYear <= 0)
                throw new ArgumentException(nameof(toYear) + " must be > 0");

            if (limit <= 0)
                throw new ArgumentException(nameof(limit) + " must be > 0");

            if (genres == null)
                throw new ArgumentNullException(nameof(genres));

            var filter = Builders<Movie>.Filter.Where(m => m.PublicationYear >= fromYear && m.PublicationYear <= toYear);

            if (genres.Count() != 0)
            {
                var genreFilter = Builders<Movie>.Filter.AnyIn("Genres", genres);
                filter = Builders<Movie>.Filter.And(filter, genreFilter);
            }

            SortDefinition<Movie> sortDefinition;

            if (orderDescByRatingCount)
                sortDefinition = Builders<Movie>.Sort.Descending(m => m.RatingCount);
            else
                sortDefinition = Builders<Movie>.Sort.Ascending(m => m.RatingCount);

            return _collection.Find(filter).Sort(sortDefinition).Skip(paginationIndex * limit).Limit(limit).ToList();

        }

        #region DistinctAPI
        public IEnumerable<string> DistinctGenres()
        {
            if (MemoryCache.Default.Contains("Genres")) // Cache this
                return (IEnumerable<string>)MemoryCache.Default.Get("Genres");

            var genres = _collection.Distinct<string>("Genres", Builders<Movie>.Filter.Empty).ToListAsync().Result;

            MemoryCache.Default.Add("Genres", genres, new CacheItemPolicy()
            {
                AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration
            });

            return genres;
        }

        public IEnumerable<int> DistinctYearsDesc()
        {
            if (MemoryCache.Default.Contains("YearsDesc")) // Cache this
                return (IEnumerable<int>)MemoryCache.Default.Get("YearsDesc");

            var yearsDesc = _collection.AsQueryable().Select(m => m.PublicationYear).Distinct().OrderByDescending(m => m).ToList();

            MemoryCache.Default.Add("YearsDesc", yearsDesc, new CacheItemPolicy()
            {
                AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration
            });

            return yearsDesc;
        }

        #endregion

        public IEnumerable<Movie> FindMoviesLikeTitleAsync(string likeTitle, int limit, bool sortByRatingCountDesc)
        {
            if (limit <= 0)
                throw new ArgumentException(nameof(limit) + " must be > 0");

            var filter = Builders<Movie>.Filter.Regex("Title", new BsonRegularExpression(likeTitle, "i"));

            SortDefinition<Movie> sortDefinition;

            if (sortByRatingCountDesc)
                sortDefinition = Builders<Movie>.Sort.Descending(m => m.RatingCount).Descending(m => m.PublicationYear);
            else
                sortDefinition = Builders<Movie>.Sort.Ascending(m => m.RatingCount);

            if (sortByRatingCountDesc)
                return _collection.Find(filter).Sort(sortDefinition).Limit(limit).ToList();
            else
                return _collection.Find(filter).Limit(limit).SortByDescending(m => m.PublicationYear).ToList();
        }

        public Movie FindMovieByImdbId(string imdbId)
        {
            return _collection.Find(m => m.IMDBId == imdbId).FirstOrDefault();
        }
    }
}