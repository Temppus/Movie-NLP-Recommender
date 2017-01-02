using MongoDB.Bson;
using MongoDB.Driver;
using MongoMigrations;
using MovieRecommender.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using MovieRecommender.Database.Models;

namespace MovieRecommender.App_Start.MongoMigrations
{
    public class Migration4 : CollectionMigration
    {
        public Migration4()
            : base("0.0.4", "movies")
        {
            Description = "Add index to IMDb id property.";
        }

        public override void BeforeMigration(IMongoCollection<BsonDocument> collection)
        {
            var dbPool = UnityConfig.GetConfiguredContainer().Resolve<MongoDbConnectionPool>();
            var movieCollection = dbPool.Database.GetCollection<Movie>("movies");

            movieCollection.Indexes.CreateOne(Builders<Movie>.IndexKeys.Descending(x => x.IMDBId));
        }
    }
}