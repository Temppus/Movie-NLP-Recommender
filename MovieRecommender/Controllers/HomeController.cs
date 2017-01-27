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
        private readonly IMovieMentionRepository _movieMentionRepository;
        private readonly IRecommender _recommender;

        public HomeController(IUserRepository userStore, IMovieMentionRepository movieMentionRepository)
        {
            _userStore = userStore;
            _movieMentionRepository = movieMentionRepository;

            _recommender = new NlpRecommender(_userStore, _movieMentionRepository);
        }

        public ActionResult Index()
        {
            ForceLayoutModel forceModel = new ForceLayoutModel();

            if (User.Identity.IsAuthenticated)
            {
                var recommendedMovies = _recommender.RecommendForUser(User.Identity.Name);
                var ForceJSON = forceModel.Compose(recommendedMovies).ToJson();

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