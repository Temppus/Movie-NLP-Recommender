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

        public IEnumerable<Movie> FilterMovies(int fromYear, int toYear, IEnumerable<string> genres, bool orderDescByRating, int limit)
        {
            if (fromYear <= 0)
                fromYear = DateTime.UtcNow.Year - 5;

            if (toYear <= 0)
                toYear = DateTime.UtcNow.Year;

            List<Movie> result = new List<Movie>();

            if (genres == null || genres.Count() == 0)
            {
                var queryable = _collection.AsQueryable()
                        .Where(x => x.PublicationYear >= fromYear && x.PublicationYear <= toYear);

                if (orderDescByRating)
                    queryable = queryable.OrderByDescending(x => x.Rating);
                else
                    queryable = queryable.OrderBy(x => x.Rating);

                return queryable.Take(limit).ToList();
            }
            else
            {
                foreach (string genre in genres)
                {
                    var queryAble = _collection.AsQueryable()
                        .Where(x => x.PublicationYear >= fromYear && x.PublicationYear <= toYear)
                        .Where(x => x.Genres.Contains(genre));

                    if (orderDescByRating)
                        queryAble = queryAble.OrderByDescending(x => x.Rating);
                    else
                        queryAble = queryAble.OrderBy(x => x.Rating);

                    result.AddRange(queryAble.ToList());
                }
            }

            return result.OrderByDescending(m => m.Rating).Take(limit);
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