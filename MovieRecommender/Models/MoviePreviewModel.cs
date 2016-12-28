﻿using AutoMapper;
using MovieRecommender.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovieRecommender.Models
{
    public class MoviePreviewModel
    {
        public IEnumerable<MoviePreview> MoviePreviews = new List<MoviePreview>();

        public MoviePreviewModel()
        {
            Genres = new List<string>();
            FromYearList = new List<int>();
            ToYearList = new List<int>();
            MoviePreviews = new List<MoviePreview>();
        }

        public MoviePreviewModel(IEnumerable<string> genres, IEnumerable<int> yearListDesc)
        {
            Genres = genres;
            FromYearList = yearListDesc;
            ToYearList = yearListDesc;

            SelectedRating = "desc";

            FromYear = yearListDesc.First() - 2;
            SelectedFromYear = FromYear;
        }

        [Display(Name = "From Year")]
        public IEnumerable<int> FromYearList { get; set; }

        public int FromYear { get; set; }

        [Display(Name = "To Year")]
        public IEnumerable<int> ToYearList { get; set; }

        public int SelectedFromYear { get; set; }
        public int SelectedToYear { get; set; }

        [Display(Name = "Order by rating")]
        public IEnumerable<string> OrderByRating { get; set; } = new List<string>() { "desc", "asc" };
        public string SelectedRating { get; set; }

        [Display(Name = "Genres")]
        public IEnumerable<string> Genres { get; set; }
        public IEnumerable<string> SelectedGenres { get; set; }
    }

    public class MoviePreview
    {
        public MoviePreview(Movie movie)
        {
            Mapper.Map(movie, this);
        }

        public int TMDBId { get; set; }
        public string IMDBId { get; set; }
        public string Title { get; set; }
        public string Director { get; set; }
        public int PublicationYear { get; set; }
        public double Rating { get; set; }
        public string Overview { get; set; }
        public string ImageURI { get; set; }
        public string TrailerImageURI { get; set; }
        public List<string> Genres { get; set; }
    }
}