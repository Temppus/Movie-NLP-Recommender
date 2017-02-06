using MongoDB.Bson;
using MongoDB.Driver;
using MovieRecommender.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender.Database.CollectionAPI
{
    public interface IMovieRepository
    {
        Movie FindMovieByImdbId(string imdbId);
        IEnumerable<Movie> FilterMovies(int fromYear, int toYear, IEnumerable<string> genres, bool orderDescByRatingCount, int limit, int paginationIndex = 0);
        IEnumerable<Movie> FindMoviesLikeTitleAsync(string likeTitle, int limit, bool sortByRatingCountDesc = true);
        IEnumerable<Movie> FindMoviesByIMDbIds(IEnumerable<string> imdbIds);
        IList<BsonDocument> FindSimilarMovies(IEnumerable<string> genres, IEnumerable<string> keywords, IEnumerable<string> exceptIMDbIds,
                                                     int limit, int fromYear, int minRatingCount, double minRating);
        IEnumerable<Movie> FindMostPopularMoviesByGenre(string genre, int fromYear, IEnumerable<string> exceptIds, int limit);
        IEnumerable<string> DistinctGenres();
        IEnumerable<int> DistinctYearsDesc();
    }
}
