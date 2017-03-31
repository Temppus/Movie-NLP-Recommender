using AspNet.Identity.MongoDB;
using MongoMigrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MovieRecommender.App_Start.MongoMigrations
{
    public class Migration1 : CollectionMigration
    {
        private static IEnumerable<IdentityRole> _roles = new List<IdentityRole>()
        {
            new IdentityRole() { Name = "admin" },
            new IdentityRole() { Name = "user" }
        };

        public Migration1() : base("0.0.1", "roles")
        {
            Description = "Initial roles for users.";
        }

        public override void AfterMigration(IMongoCollection<BsonDocument> collection)
        {
            collection.InsertMany(_roles.Select(r => r.ToBsonDocument()));
        }
    }
}