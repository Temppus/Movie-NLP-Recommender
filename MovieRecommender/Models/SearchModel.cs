using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Models
{
    public class SearchModel
    {
        public int SelectedFromYear { get; set; }
        public int SelectedToYear { get; set; }
        public string SelectedRating { get; set; }
        public List<string> SelectedGenres { get; set; }
        public int PaginationIndex { get; set; }
    }
}