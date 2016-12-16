using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MovieRecommender.Startup))]
namespace MovieRecommender
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
