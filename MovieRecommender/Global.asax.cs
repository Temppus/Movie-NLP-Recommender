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
using MovieRecommender.StartupHooks;
using Serilog.Core;

namespace MovieRecommender
{
    public class MvcApplication : System.Web.HttpApplication
    {
        // Register all possible on start application hooks
        private static List<IRunOnApplicationStart> onStartHooks = new List<IRunOnApplicationStart>
        {
                new CheckDbVersionOnStartup(),
                new IntegrityManager()
        };

        public MvcApplication()
        {

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Mapper.Initialize(cfg => {
                cfg.CreateMap<Movie, MoviePreview>();
                cfg.CreateMap<Movie, MovieDetailModel>();
                cfg.CreateMap<ActorsInfo, ActorModel>();
            });

            onStartHooks.ForEach(x => x.Start());
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            // Log all exceptions
            var logger = UnityConfig.GetConfiguredContainer().Resolve<Logger>();
            logger.Error(exception, "ApplicationError");
        }
    }
}
