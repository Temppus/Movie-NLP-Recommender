using MongoMigrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MovieRecommender.Models;
using MovieRecommender.App_Start.IdentityConfiguration;
using Microsoft.Practices.Unity;
using AspNet.Identity.MongoDB;

namespace MovieRecommender.App_Start.MongoMigrations
{
    public class Migration2 : CollectionMigration
    {
        private const string AdminAccountName = "admin";
        private const string AdminAccountPassword = "admin";

        public Migration2()
            : base("0.0.2", "users")
        {
            Description = "Insert admin user to database.";
        }

        public override void AfterMigration(IMongoCollection<BsonDocument> collection)
        {
            var userManager = UnityConfig.GetConfiguredContainer().Resolve<ApplicationUserManager>();
            var roleManager = UnityConfig.GetConfiguredContainer().Resolve<ApplicationRoleManager>();

            var roles = roleManager.Roles;

            ApplicationUser adminUser = new ApplicationUser()
            {
                UserName = AdminAccountName,
                PasswordHash = userManager.PasswordHasher.HashPassword(AdminAccountPassword),
                Roles = roles.Select(r => r.Name).ToList(),
                SecurityStamp = Guid.NewGuid().ToString() // EF requiring this
            };

            var bsonUser = adminUser.ToBsonDocument();
            collection.InsertOne(bsonUser);
        }
    }
}