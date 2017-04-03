using MongoDB.Bson;
using MongoDB.Driver;
using MongoMigrations;
using MovieRecommender.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using MovieRecommender.Database;

namespace MovieRecommender.App_Start.MongoMigrations
{
    public class Migration3 : CollectionMigration
    {
        public Migration3()
            : base("0.0.3", "movies")
        {
            Description = "Add indexes to PublicationYear and IMDBId property.";
        }

        public override void BeforeMigration(IMongoCollection<BsonDocument> collection)
        {
            var dbPool = UnityConfig.GetConfiguredContainer().Resolve<MongoDbConnectionPool>();
            var movieCollection = dbPool.Database.GetCollection<Movie>("movies");

            movieCollection.Indexes.CreateOne(Builders<Movie>.IndexKeys.Descending(x => x.PublicationYear));
            movieCollection.Indexes.CreateOne(Builders<Movie>.IndexKeys.Ascending(x => x.PublicationYear));
            movieCollection.Indexes.CreateOne(Builders<Movie>.IndexKeys.Descending(x => x.IMDBId));
        }
    }
}