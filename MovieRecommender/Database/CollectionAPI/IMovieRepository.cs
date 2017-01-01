﻿using MongoDB.Driver;
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
        IEnumerable<Movie> FilterMovies(int fromYear, int toYear, IEnumerable<string> genres, bool orderDescByRatingCount, int limit, int paginationIndex = 0);
        IEnumerable<Movie> FindMoviesLikeTitleAsync(string likeTitle, int limit, bool sortByRatingCountDesc = true);
        IEnumerable<string> DistinctGenres();
        IEnumerable<int> DistinctYearsDesc();

    }
}
