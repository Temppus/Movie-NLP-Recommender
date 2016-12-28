using MovieRecommender.App_Start;
using MovieRecommender.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using AutoMapper;
using MovieRecommender.Database.Models;
using MovieRecommender.Models;

namespace MovieRecommender
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private readonly IRepositoryManager _db;

        // Register all possible on start application hooks
        private static List<IRunOnApplicationStart> onStartHooks = new List<IRunOnApplicationStart>
        {
                new CheckDbVersionOnStartup()
        };

        public MvcApplication() : this(UnityConfig.GetConfiguredContainer().Resolve<RepositoryManager>())
        {

        }

        public MvcApplication(IRepositoryManager repoManager)
        {
            _db = repoManager;
        }



        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Mapper.Initialize(cfg => {
                cfg.CreateMap<Movie, MoviePreview>();
            });

            onStartHooks.ForEach(x => x.Start());
        }
    }

    public class CheckDbVersionOnStartup : IRunOnApplicationStart
    {
        public void Start()
        {
            DataBaseConfiguration.ApplyMigrations();
        }
    }

    public interface IRunOnApplicationStart
    {
        void Start();
    }
}
