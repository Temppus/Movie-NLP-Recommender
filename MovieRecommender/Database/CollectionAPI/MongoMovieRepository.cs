using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Driver;
using MovieRecommender.Database.Models;
using System.Runtime.Caching;

namespace MovieRecommender.Database.CollectionAPI
{
    public class MongoMovieRepository : IMovieRepository
    {
        private IMongoCollection<Movie> _collection;

        public MongoMovieRepository(MongoDbConnectionPool dbPool)
        {
            _collection = dbPool.Database.GetCollection<Movie>("movies");
        }

        public IEnumerable<Movie> FilterMovies(int fromYear, int toYear, IEnumerable<string> genres, bool orderDescByRating, int limit, int paginationIndex)
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

            var sortedQuery = orderDescByRating
                                            ? _collection.Find(filter).Skip(paginationIndex * limit).Limit(limit).SortByDescending(m => m.Rating)
                                            : _collection.Find(filter).Skip(paginationIndex * limit).Limit(limit).SortBy(m => m.Rating);

            return sortedQuery.ToList();
        }

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
    }
}