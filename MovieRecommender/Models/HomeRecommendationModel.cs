using MovieRecommender.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovieRecommender.Models
{
    public class HomeRecommendationModel
    {
        public HomeRecommendationModel()
        {
            Genres = new List<string>();
            FromYearList = new List<int>();
            ToYearList = new List<int>();
        }

        public HomeRecommendationModel(IEnumerable<string> genres, IEnumerable<int> yearListDesc)
        {
            Genres = genres;
            FromYearList = yearListDesc;
            ToYearList = yearListDesc;

            if (yearListDesc.Count() != 0)
            {
                SelectedFromYear = yearListDesc.Max() - 20;
                SelectedToYear = yearListDesc.Max();
            }

            SelectedGenres = new List<string>();
        }

        public int MoviesRemainingCount { get; set; } = 15;
        public int RatedMoviesCount { get; set; } = 0;

        public bool ColdStartDone { get; set; } = false;

        public IList<MovieSuggestionModel> RecommendedMovies { get; set; } = new List<MovieSuggestionModel>();

        [Display(Name = "From Year")]
        public IEnumerable<int> FromYearList { get; set; }
        public int SelectedFromYear { get; set; }

        [Display(Name = "To Year")]
        public IEnumerable<int> ToYearList { get; set; }
        public int SelectedToYear { get; set; }

        [Display(Name = "Genres")]
        public IEnumerable<string> Genres { get; set; }
        public IEnumerable<string> SelectedGenres { get; set; }

        [Display(Name = "Min rating")]
        public IEnumerable<int> MinRatingsDesc { get; set; } = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public int SelectedMinRating { get; set; } = 7;
    }
}