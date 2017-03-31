using Microsoft.Owin.Security;
using MovieRecommender.App_Start.IdentityConfiguration;
using MovieRecommender.Database;
using MovieRecommender.Database.CollectionAPI;
using MovieRecommender.Database.Models;
using MovieRecommender.Models;
using MovieRecommender.Recommending;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MovieRecommender.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private const int _minLikedMovies = 10;
        private const int _minExperimentMovies = 15;

        private readonly IUserRepository _userStore;
        private readonly IMovieRepository _movieStore;
        private readonly IReviewRepository _reviewStore;
        private readonly IMovieMentionRepository _movieMentionRepository;
        private readonly IRecommender _recommender;

        private readonly IRepositoryManager _repoManager;
        private readonly IAuthenticationManager _authManager;
        private readonly ApplicationUserManager _userManager;
        private readonly SignInHelper _signInHelper;

        public HomeController(
            IUserRepository userStore,
            IMovieMentionRepository movieMentionRepository,
            IMovieRepository movieStore,
            IReviewRepository reviewStore,
            ApplicationUserManager userManager, IAuthenticationManager authManager, IRepositoryManager repoManager)
        {
            _userStore = userStore;
            _movieMentionRepository = movieMentionRepository;
            _movieStore = movieStore;
            _reviewStore = reviewStore;

            _repoManager = repoManager;
            _userManager = userManager;
            _authManager = authManager;

            _signInHelper = new SignInHelper(userManager, authManager, repoManager);
            _recommender = new ContentBasedRecommender(_userStore, _movieStore);
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            HomeRecommendationModel model = new HomeRecommendationModel(_movieStore.DistinctGenres(), _movieStore.DistinctYearsDesc());

            if (!Request.IsAuthenticated)
            {
                return View(model);
            }

            ISet<string> exceptIds = new HashSet<string>();

            var expData = _userStore.GetExperimentDataForUser(User.Identity.Name);
            int experimentMoviesCount = 0;

            if (expData != null)
            {
                exceptIds.UnionWith(expData.WouldWatchIds());
                exceptIds.UnionWith(expData.WouldNotWatchIds());

                experimentMoviesCount = expData.WouldWatchIds().Count() + expData.WouldNotWatchIds().Count();
                model.RatedMoviesCount = experimentMoviesCount;
            }

            var likedMovieIds = _userStore.FindLikedMovieIds(User.Identity.Name);

            if (likedMovieIds.Count() < _minLikedMovies)
                return RedirectToAction("ColdStart");

            model.ColdStartDone = true;

            var likedMovies = _movieStore.FindMoviesByIMDbIds(likedMovieIds);

            int minYear = likedMovies.Select(m => m.PublicationYear).Min() - 5;
            int maxYear = _movieStore.DistinctYearsDesc().Max();
            double minRating = likedMovies.Select(m => m.Rating).Min();
            var genres = likedMovies.Select(m => m.Genres);

            ISet<string> genreSet = new HashSet<string>();

            foreach (var movieGenres in genres)
            {
                genreSet.UnionWith(movieGenres);
            }

            model.RecommendedMovies = _recommender.RecommendForUser(minRating: minRating,
                                                                    fromYear: minYear,
                                                                    toYear: maxYear,
                                                                    genres: genreSet,
                                                                    limit: _minExperimentMovies,
                                                                    userName: User.Identity.Name,
                                                                    exceptIds: exceptIds)
                                                                    .ToList();

            int cutoffCount = _minExperimentMovies - experimentMoviesCount;

            // We did not rate enough movies
            if (cutoffCount > 0)
            {
                model.MoviesRemainingCount = cutoffCount;
                model.RecommendedMovies = model.RecommendedMovies.Take(10).ToList();
            }
            // We rated enough movies
            else
            {
                model.MoviesRemainingCount = 0;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Search(HomeRecommendationModel model)
        {
            bool noGenresSelected = model.SelectedGenres == null;

            model.ColdStartDone = true;
            model.RecommendedMovies = _recommender.RecommendForUser(minRating: model.SelectedMinRating,
                                                                    fromYear: model.SelectedFromYear,
                                                                    toYear: model.SelectedToYear,
                                                                    genres: noGenresSelected ? _movieStore.DistinctGenres() : model.SelectedGenres,
                                                                    limit: 100,
                                                                    userName: User.Identity.Name)
                                                                    .ToList();

            if (noGenresSelected)
                model.SelectedGenres = new List<string>(); // model binder is binding empty collections as nulls

            return View("Index", model);
        }


        [AllowAnonymous]
        public ActionResult ColdStart()
        {
            ColdStartModel model = new ColdStartModel();

            if (!Request.IsAuthenticated)
                return View(model);

            var likedMovieIds = _userStore.FindLikedMovieIds(User.Identity.Name);

            var genres = _movieStore.DistinctGenres().ToList();

            genres.Remove("TV Movie");
            genres.Remove("Music");
            genres.Remove("Documentary");
            genres.Remove("Foreign");
            genres.Remove("Animation");
            genres.Remove("History");

            HashSet<string> uniqueMovieIds = new HashSet<string>();

            foreach (string genre in genres)
            {
                var exceptIds = likedMovieIds.Concat(uniqueMovieIds);
                var movies = _movieStore.FindMostPopularMoviesByGenres(new List<string> { genre }, 2000, exceptIds, 100);

                uniqueMovieIds.UnionWith(movies.Select(m => m.IMDBId));

                model.MoviesDic.Add(genre, movies.OrderByDescending(m => m.RatingCount).Take(50).OrderByDescending(x => x.Rating * 25000 + x.RatingCount));
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public JsonResult CheckLikedMovies(string userName)
        {
            int likedMovies = _userStore.FindLikedMovieIds(userName).Count();

            if (likedMovies < _minLikedMovies)
            {
                return Json(new { Ok = false, Message = $"You must like at least {_minLikedMovies} movies to proceed. You liked only {likedMovies} so far." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Ok = true, Message = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> ExperimentHandler(ExperimentResultModel model)
        {
            string userName = User.Identity.Name;

            _userStore.UserLikedMovies(userName, model.WatchedIds());
            _userStore.SetOrAddExperiment(userName, model);

            // Reload identity cookie
            var user = _userManager.FindByNameAsync(userName).Result;
            await _signInHelper.SignInAsync(user, false, false);

            return Json(true);
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
