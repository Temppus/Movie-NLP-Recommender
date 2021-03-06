﻿using MovieRecommender.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender.Database.CollectionAPI
{
    public interface IMovieMentionRepository
    {
        IEnumerable<MovieMention> GetMovieMentionsForMovies(IEnumerable<string> movieIds);
    }
}
