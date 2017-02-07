using AutoMapper;
using MongoDB.Driver;
using MovieRecommender.App_Start.IdentityConfiguration;
using MovieRecommender.Database;
using MovieRecommender.Database.CollectionAPI;
using MovieRecommender.Database.Models;
using MovieRecommender.Models;
using MovieRecommender.Recommending;
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
    [Authorize]
    public class MovieController : Controller
    {
        // Identity
        private readonly ApplicationUserManager _userManager;

        // Our repository API
        private readonly IMovieRepository _movieStore;
        private readonly IReviewRepository _reviewStore;
        private readonly IUserRepository _userStore;
        private readonly IMovieMentionRepository _movieMentionRepository;
        private readonly IRecommender _recommender;

        private const int _movieLimit = 10;

        /// <summary>
        /// Parameters injected via unity IoC container
        /// </summary>
        public MovieController(ApplicationUserManager userManager, IUserRepository userStore, IMovieRepository movieStore,
                                IReviewRepository reviewStore, IMovieMentionRepository movieMentionRepository)
        {
            _userStore = userStore;
            _userManager = userManager;
            _movieStore = movieStore;
            _reviewStore = reviewStore;
            _movieMentionRepository = movieMentionRepository;

            //_recommender = new NlpRecommender(_userStore, _movieMentionRepository);
            _recommender = new ContentBasedRecommender(_userStore, _movieStore);
        }

        // GET: Movie
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            var model = new MoviePreviewModel(_movieStore.DistinctGenres(), _movieStore.DistinctYearsDesc());
            var movies = GetMoviesByModelSearchQuery(model, _movieLimit);

            model.MoviePreviews = movies.Select(m => new MoviePreview(m)).ToList();
            MapMovieLikes(model);

            if (Request.IsAuthenticated)
            {
                var recommendedMovies = _recommender.RecommendForUser(User.Identity.Name);
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Browse()
        {
            var model = new MoviePreviewModel(_movieStore.DistinctGenres(), _movieStore.DistinctYearsDesc());
            var movies = GetMoviesByModelSearchQuery(model, _movieLimit);

            model.MoviePreviews = movies.Select(m => new MoviePreview(m)).ToList();
            MapMovieLikes(model);

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Search(MoviePreviewModel model)
        {
            model.SelectedGenres = model.SelectedGenres ?? new List<string>(); // model binder is binding empty collections as nulls

            var movies = GetMoviesByModelSearchQuery(model, _movieLimit);
            model.MoviePreviews = movies.Select(m => new MoviePreview(m)).ToList();
            MapMovieLikes(model);

            return View("Index", model);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult PaginationSearch(SearchModel model)
        {
            model.SelectedGenres = model.SelectedGenres ?? new List<string>(); // model binder is binding empty collections as nulls

            if (model.SelectedFromYear < 0 || model.SelectedToYear < 0 || model.PaginationIndex < 0)
                return Json(null);

            var movies = _movieStore.FilterMovies(model.SelectedFromYear,
                                                    model.SelectedToYear,
                                                    model.SelectedGenres,
                                                    model.SelectedRating == "desc",
                                                    _movieLimit,
                                                    model.PaginationIndex);

            var moviePreviews = movies.Select(m => new MoviePreview(m)).ToList();

            if (Request.IsAuthenticated)
            {
                var likeMappings = _userStore.GetUserLikedMovieMappings(User.Identity.Name, moviePreviews.Select(p => p.IMDBId));

                foreach (var preview in moviePreviews)
                {
                    preview.IsLikedByUser = likeMappings[preview.IMDBId];
                }
            }

            return Json(moviePreviews);
        }

        [HttpGet]
        [AllowAnonymous]
        public string SearchMoviesLike(string title)
        {
            var movies = _movieStore.FindMoviesLikeTitleAsync(title, 10, true);
            var json = JsonConvert.SerializeObject(new QuickSearchResponse(movies));
            return json;
        }

        #region #Helpers
        private IEnumerable<Movie> GetMoviesByModelSearchQuery(MoviePreviewModel model, int limit)
        {
            return _movieStore.FilterMovies(model.SelectedFromYear, model.SelectedToYear, model.SelectedGenres, model.SelectedRating == "desc", limit);
        }

        private void MapMovieLikes(MoviePreviewModel model)
        {
            if (Request.IsAuthenticated)
            {
                var likeMappings = _userStore.GetUserLikedMovieMappings(User.Identity.Name, model.MoviePreviews.Select(p => p.IMDBId));

                foreach (var preview in model.MoviePreviews)
                {
                    preview.IsLikedByUser = likeMappings[preview.IMDBId];
                }
            }
        }
        #endregion

        [HttpGet, OutputCache(NoStore = true, Duration = 1)]
        [AllowAnonymous]
        public ActionResult Details(string imdbId)
        {
            Movie movie = _movieStore.FindMovieByImdbId(imdbId);

            if (movie == null)
                return View();

            MovieDetailModel model = new MovieDetailModel();
            Mapper.Map(movie, model);

            model.Reviews = _reviewStore.FindReviewsByReviewId(movie.ReviewId);
            model.IsLikedMovie = false;

            if (Request.IsAuthenticated)
            {
                model.IsLikedMovie = _userStore.CheckIfUserLikedMovie(User.Identity.Name, imdbId);
                model.MovieSuggestions = _recommender.RecommendForUserByMovie(User.Identity.Name, imdbId);
            }

            return View("Detail", model);
        }

        [HttpPost]
        public JsonResult LikeHandler(LikeModel model)
        {
            string userName = User.Identity.Name;

            if (model.IsLike)
            {
                _userStore.UserLikedMovie(userName, model.IMDbId);
            }
            else
            {
                _userStore.UserUnlikedMovie(userName, model.IMDbId);
            }

            return Json(true);
        }

        [HttpPost]
        public JsonResult NotInterestedHandler(InterestModel model)
        {
            string userName = User.Identity.Name;

            if (model.IsNotInterested)
            {
                _userStore.AddMovieToNotInterested(userName, model.IMDbId);
            }
            else // user is interested in this movie again
            {
                _userStore.RemoveMovieFromNotInterested(userName, model.IMDbId);
            }

            return Json(true);
        }
    }
}
