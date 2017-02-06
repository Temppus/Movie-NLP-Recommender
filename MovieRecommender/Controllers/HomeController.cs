using MovieRecommender.Database.CollectionAPI;
using MovieRecommender.Database.Models;
using MovieRecommender.Models;
using MovieRecommender.Recommending;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieRecommender.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository _userStore;
        private readonly IMovieRepository _movieStore;
        private readonly IMovieMentionRepository _movieMentionRepository;
        private readonly IRecommender _recommender;

        public HomeController(IUserRepository userStore, IMovieMentionRepository movieMentionRepository, IMovieRepository movieStore)
        {
            _userStore = userStore;
            _movieMentionRepository = movieMentionRepository;
            _movieStore = movieStore;

            _recommender = new ContentBasedRecommender(_userStore, _movieStore);
        }

        /*public ActionResult Index()
        {
            ForceLayoutModel forceModel = new ForceLayoutModel();

            if (Request.IsAuthenticated)
            {
                var recommendedMovies = _recommender.RecommendForUser(User.Identity.Name);
                var ForceJSON = forceModel.Compose(new List<MovieMention>()).ToJson();

                HomeViewModel homeModel = new HomeViewModel()
                {
                    ForceModelJson = ForceJSON
                };

                return View(homeModel);
            }
            else
            {
                HomeViewModel homeModel = new HomeViewModel()
                {
                    ForceModelJson = forceModel.ToJson()
                };

                return View(homeModel);
            }
        }*/

        public ActionResult Index()
        {
            ColdStartModel model = new ColdStartModel();

            if (!Request.IsAuthenticated)
                return View(model);

            var likedMovieIds = _userStore.FindLikedMovieIds(User.Identity.Name);

            var genres = _movieStore.DistinctGenres();

            HashSet<string> uniqueMovieIds = new HashSet<string>();

            foreach (string genre in genres)
            {
                var exceptIds = likedMovieIds.Concat(uniqueMovieIds);
                var movies = _movieStore.FindMostPopularMoviesByGenre(genre, 2000, exceptIds, 100);

                uniqueMovieIds.UnionWith(movies.Select(m => m.IMDBId));

                model.MoviesDic.Add(genre, movies);
            }

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}