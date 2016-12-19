using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime;
using System.Web;

namespace MovieRecommender.Database
{
    public class MongoDbConnectionPool
    {
        private readonly IMongoDatabase database;

        private static bool _instantiated = false;

        /// <summary>
        /// Should be called only once, from unity IoC container
        /// </summary>
        public MongoDbConnectionPool()
        {
            if (_instantiated)
                throw new InvalidOperationException("Connection pool was instantiated more than once. Use IoC container only.");

            MongoClient client = new MongoClient(ConfigurationManager.ConnectionStrings["Mongo"].ConnectionString);
            database = client.GetDatabase("MovieRecommenderDb");
            _instantiated = true;
        }

        public IMongoDatabase Database { get { return database; } }
    }
}