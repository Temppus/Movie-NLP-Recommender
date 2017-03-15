﻿using MovieRecommender.Database.CollectionAPI;
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
    [Authorize]
    public class HomeController : Controller
    {
        private const int _minLikedMovies = 10;

        private readonly IUserRepository _userStore;
        private readonly IMovieRepository _movieStore;
        private readonly IReviewRepository _reviewStore;
        private readonly IMovieMentionRepository _movieMentionRepository;
        private readonly IRecommender _recommender;

        public HomeController(IUserRepository userStore, IMovieMentionRepository movieMentionRepository, IMovieRepository movieStore, IReviewRepository reviewStore)
        {
            _userStore = userStore;
            _movieMentionRepository = movieMentionRepository;
            _movieStore = movieStore;
            _reviewStore = reviewStore;

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

            if (_userStore.FindLikedMovieIds(User.Identity.Name).Count() < _minLikedMovies)
                return RedirectToAction("ColdStart");

            model.ColdStartDone = true;

            model.RecommendedMovies = _recommender.RecommendForUser(minRating: model.SelectedMinRating,
                                                                    fromYear: model.SelectedFromYear,
                                                                    toYear: model.SelectedToYear,
                                                                    genres: _movieStore.DistinctGenres(),
                                                                    limit: 100,
                                                                    userName: User.Identity.Name)
                                                                    .ToList();


            var movies = _movieStore.FindMoviesByIMDbIds(model.RecommendedMovies.Select(m => m.IMDBId));

            foreach (var movie in movies)
            {
                var reviews = _reviewStore.FindReviewsByReviewId(movie.ReviewId).OrderByDescending(r => r.Rating).Take(5);

                var explanation = new Explanation();

                foreach (var review in reviews)
                {
                    explanation.SentimentHolders.Add(new SentimentHolder(sentence: review.Title, score: review.Rating));
                }

                model.RecommendedMovies.First(m => m.IMDBId == movie.IMDBId).Explanation = explanation;
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

            HashSet<string> uniqueMovieIds = new HashSet<string>();

            foreach (string genre in genres)
            {
                var exceptIds = likedMovieIds.Concat(uniqueMovieIds);
                var movies = _movieStore.FindMostPopularMoviesByGenres(new List<string> { genre }, 2000, exceptIds, 100);

                uniqueMovieIds.UnionWith(movies.Select(m => m.IMDBId));

                model.MoviesDic.Add(genre, movies);
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
