using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Driver;
using MovieRecommender.Database.Models;
using System.Runtime.Caching;
using MongoDB.Bson;
using MovieRecommender.Extensions;

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
            fromYear.ThrowIfNegativeOrZero(nameof(fromYear));
            toYear.ThrowIfNegativeOrZero(nameof(toYear));
            limit.ThrowIfNegativeOrZero(nameof(limit));
            genres.ThrowIfNull(nameof(genres));

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
            limit.ThrowIfNegativeOrZero(nameof(limit));

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

        public IEnumerable<Movie> FindMoviesByIMDbIds(IEnumerable<string> imdbIds)
        {
            imdbIds.ThrowIfNull(nameof(imdbIds));

            var filter = Builders<Movie>.Filter.In(m => m.IMDBId, imdbIds);

            var movies = _collection.Find(filter).ToList();
            return movies;
        }

        public IList<BsonDocument> FindSimilarMovies(IEnumerable<string> genres, IEnumerable<string> keywords, IEnumerable<string> exceptIMDbIds,
                                                            int limit, int fromYear, int toYear, int minRatingCount, double minRating)
        {
            IEnumerable<FilterDefinition<Movie>> filters = new List<FilterDefinition<Movie>>
            {
                Builders<Movie>.Filter.AnyIn(m => m.Genres, genres),
                Builders<Movie>.Filter.AnyIn(m => m.KeyWords, keywords),
                Builders<Movie>.Filter.Nin(m => m.IMDBId, exceptIMDbIds),
                Builders<Movie>.Filter.Where(m => m.PublicationYear >= fromYear),
                Builders<Movie>.Filter.Where(m => m.PublicationYear <= toYear),
                Builders<Movie>.Filter.Where(m => m.RatingCount >= minRatingCount),
                Builders<Movie>.Filter.Where(m => m.Rating > minRating)
            };

            var groupByExpression = new Dictionary<string, BsonDocument>();
            string sumKey = "matches";

            // group by id of film
            groupByExpression.Add("_id", new BsonDocument("_id", "$_id"));
            // count sum of keyword matches
            groupByExpression.Add(sumKey, new BsonDocument("$sum", 1));

            // include other properties in grouping output document
            groupByExpression.Add(nameof(Movie.Title), new BsonDocument("$first", "$" + nameof(Movie.Title)));
            groupByExpression.Add(nameof(Movie.PublicationYear), new BsonDocument("$first", "$" + nameof(Movie.PublicationYear)));
            groupByExpression.Add(nameof(Movie.IMDBId), new BsonDocument("$first", "$" + nameof(Movie.IMDBId)));
            groupByExpression.Add(nameof(Movie.ImageURI), new BsonDocument("$first", "$" + nameof(Movie.ImageURI)));
            groupByExpression.Add(nameof(Movie.Rating), new BsonDocument("$first", "$" + nameof(Movie.Rating)));
            groupByExpression.Add(nameof(Movie.RatingCount), new BsonDocument("$first", "$" + nameof(Movie.RatingCount)));
            groupByExpression.Add(nameof(Movie.Overview), new BsonDocument("$first", "$" + nameof(Movie.Overview)));
            groupByExpression.Add(nameof(Movie.Director), new BsonDocument("$first", "$" + nameof(Movie.Director)));
            groupByExpression.Add(nameof(Movie.Genres), new BsonDocument("$first", "$" + nameof(Movie.Genres)));
            groupByExpression.Add(nameof(Movie.ReviewId), new BsonDocument("$first", "$" + nameof(Movie.ReviewId)));

            return _collection.Aggregate().Match(Builders<Movie>.Filter.And(filters))
                    .Unwind(m => m.KeyWords)
                    .Match(Builders<BsonDocument>.Filter.AnyIn(nameof(Movie.KeyWords), keywords))
                    .Group(new BsonDocument(groupByExpression))
                    .Sort(new BsonDocument(sumKey, -1))
                    .Limit(limit)
                    .ToList();
        }

        public IEnumerable<Movie> FindMostPopularMoviesByGenres(IEnumerable<string> genres, int fromYear, IEnumerable<string> exceptIds, int limit)
        {
            genres.ThrowIfNull(nameof(genres));
            exceptIds.ThrowIfNull(nameof(exceptIds));
            limit.ThrowIfNegativeOrZero(nameof(limit));
            fromYear.ThrowIfNegativeOrZero(nameof(fromYear));

            IEnumerable<FilterDefinition<Movie>> filters = new List<FilterDefinition<Movie>>
            {
                Builders<Movie>.Filter.AnyIn(m => m.Genres, genres),
                Builders<Movie>.Filter.Nin(m => m.IMDBId, exceptIds),
                Builders<Movie>.Filter.Where(m => m.PublicationYear >= fromYear),
            };

            var movies = _collection.Find(Builders<Movie>.Filter.And(filters))
                                    .SortByDescending(m => m.RatingCount)
                                    .Limit(limit)
                                    .ToList();
            return movies;
        }
    }
}
