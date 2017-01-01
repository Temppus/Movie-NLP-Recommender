using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.AspNet.Identity;
using MovieRecommender.Models;
using AspNet.Identity.MongoDB;
using System.Web;
using Microsoft.Owin.Security;
using MovieRecommender.Database;
using MovieRecommender.App_Start.IdentityConfiguration;
using MovieRecommender.Database.CollectionAPI;

namespace MovieRecommender.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            container.RegisterType<ApplicationUserManager>();
            container.RegisterType<ApplicationRoleManager>();
            container.RegisterType<SignInHelper>();

            var mongoPool = new MongoDbConnectionPool();

            container.RegisterInstance(mongoPool); // Rgister "singleton" like object dependency

            container.RegisterType<IMovieRepository, MongoMovieRepository>(new InjectionConstructor(container.Resolve<MongoDbConnectionPool>()));
            container.RegisterType<IReviewRepository, MongoReviewRepository>(new InjectionConstructor(container.Resolve<MongoDbConnectionPool>()));

            container.RegisterType<ApplicationIdentityContext>(new InjectionConstructor(container.Resolve<MongoDbConnectionPool>()));
            container.RegisterType<IRepositoryManager, RepositoryManager>(new InjectionConstructor(container.Resolve<MongoDbConnectionPool>()));

            container.RegisterType<IAuthenticationManager>(
                new InjectionFactory(c => HttpContext.Current.GetOwinContext().Authentication));

            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(new InjectionFactory((c) =>
            {
                return new UserStore<ApplicationUser>(container.Resolve<ApplicationIdentityContext>().Users);
            }));

            container.RegisterType<IRoleStore<IdentityRole, string>, RoleStore<IdentityRole>>(new InjectionFactory((c) =>
            {
                return new RoleStore<IdentityRole>(container.Resolve<ApplicationIdentityContext>().Roles);
            }));
        }
    }
}
