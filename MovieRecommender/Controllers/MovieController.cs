using MongoDB.Driver;
using MovieRecommender.App_Start.IdentityConfiguration;
using MovieRecommender.Database;
using MovieRecommender.Database.CollectionAPI;
using MovieRecommender.Database.Models;
using MovieRecommender.Models;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Parameters injected via unity IoC container
        /// </summary>
        public MovieController(ApplicationUserManager userManager, IRepositoryManager repoManager, IMovieRepository movieStore)
        {
            _repoManager = repoManager;
            _userManager = userManager;
            _movieStore = movieStore;
        }

        // GET: Movie
        public ActionResult Index()
        {
            var model = new MoviePreviewModel(_movieStore.DistinctGenres(), _movieStore.DistinctYearsDesc());
            var movies = GetMoviesByModelSearchQuery(model, 20);
            model.MoviePreviews = movies.Select(m => new MoviePreview(m));
            return View(model);
        }


        [HttpPost]
        public ActionResult Search(MoviePreviewModel model)
        {
            var movies = GetMoviesByModelSearchQuery(model, 20);
            model.MoviePreviews = movies.Select(m => new MoviePreview(m));

            return View("Index", model);
        }

        private IEnumerable<Movie> GetMoviesByModelSearchQuery(MoviePreviewModel model, int limit)
        {
            return _movieStore.FilterMovies(model.SelectedFromYear, model.SelectedToYear, model.SelectedGenres, model.SelectedRating == "desc", limit);
        }

        // GET: Movie/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Movie/Details/5
        public ActionResult Filter(int id)
        {
            return View();
        }
    }
}
