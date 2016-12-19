using AspNet.Identity.MongoDB;
using MongoDB.Driver;
using MovieRecommender.Database;
using MovieRecommender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MovieRecommender.App_Start
{
    public class ApplicationIdentityContext
    {
        public ApplicationIdentityContext(MongoDbConnectionPool pool)
        {
            Users = pool.Database.GetCollection<ApplicationUser>("users");
            Roles = pool.Database.GetCollection<IdentityRole>("roles");

            IndexChecks.EnsureUniqueIndexOnUserName(Users);
            IndexChecks.EnsureUniqueIndexOnRoleName(Roles);
        }

        public IMongoCollection<IdentityRole> Roles { get; set; }

        public IMongoCollection<ApplicationUser> Users { get; set; }
    }
}