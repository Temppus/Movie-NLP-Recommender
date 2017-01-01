using AutoMapper;
using MongoDB.Driver;
using MovieRecommender.App_Start.IdentityConfiguration;
using MovieRecommender.Database;
using MovieRecommender.Database.CollectionAPI;
using MovieRecommender.Database.Models;
using MovieRecommender.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MovieRecommender.Controllers
{
    public class MovieController : Controller
    {
        private readonly IRepositoryManager _repoManager;
        private readonly ApplicationUserManager _userManager;
        private readonly IMovieRepository _movieStore;
        private readonly IReviewRepository _reviewStore;

        private const int _movieLimit = 10;

        /// <summary>
        /// Parameters injected via unity IoC container
        /// </summary>
        public MovieController(ApplicationUserManager userManager, IRepositoryManager repoManager, IMovieRepository movieStore, IReviewRepository reviewStore)
        {
            _repoManager = repoManager;
            _userManager = userManager;
            _movieStore = movieStore;
            _reviewStore = reviewStore;
        }

        // GET: Movie
        public ActionResult Index()
        {
            var model = new MoviePreviewModel(_movieStore.DistinctGenres(), _movieStore.DistinctYearsDesc());
            var movies = GetMoviesByModelSearchQuery(model, _movieLimit);
            model.MoviePreviews = movies.Select(m => new MoviePreview(m));
            return View(model);
        }


        [HttpPost]
        public ActionResult Search(MoviePreviewModel model)
        {
            model.SelectedGenres = model.SelectedGenres ?? new List<string>(); // model binder is binding empty collections as nulls

            var movies = GetMoviesByModelSearchQuery(model, _movieLimit);
            model.MoviePreviews = movies.Select(m => new MoviePreview(m));

            return View("Index", model);
        }

        [HttpPost]
        public JsonResult PaginationSearch(SearchModel model)
        {
            model.SelectedGenres = model.SelectedGenres ?? new List<string>(); // model binder is binding empty collections as nulls

            if (model.SelectedGenres == null || model.SelectedFromYear < 0 || model.SelectedToYear < 0 || model.PaginationIndex < 0)
                return Json(null);

            model.SelectedGenres = model.SelectedGenres ?? new List<string>(); // model binder is binding empty collections as nulls

            var movies = _movieStore.FilterMovies(model.SelectedFromYear,
                                                    model.SelectedToYear,
                                                    model.SelectedGenres,
                                                    model.SelectedRating == "desc",
                                                    _movieLimit,
                                                    model.PaginationIndex);

            var moviePreviews = movies.Select(m => new MoviePreview(m));

            return Json(moviePreviews);
        }

        [HttpGet]
        public string SearchMoviesLike(string title)
        {
            var movies = _movieStore.FindMoviesLikeTitleAsync(title, 10, true);
            var json = JsonConvert.SerializeObject(new QuickSearchResponse(movies));
            return json;
        }

        private IEnumerable<Movie> GetMoviesByModelSearchQuery(MoviePreviewModel model, int limit)
        {
            return _movieStore.FilterMovies(model.SelectedFromYear, model.SelectedToYear, model.SelectedGenres, model.SelectedRating == "desc", limit);
        }

        // GET: Movie/Details/5
        public ActionResult Details(string imdbId)
        {
            Movie movie = _movieStore.FindMovieByImdbId(imdbId);

            if (movie == null)
                return View();

            MovieDetailModel model = new MovieDetailModel();
            Mapper.Map(movie, model);

            model.Reviews = _reviewStore.FindReviewsByReviewId(movie.ReviewId);

            return View("Detail", model);
        }
    }
}
