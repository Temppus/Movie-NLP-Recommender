using MovieRecommender.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using MongoMigrations;
using MovieRecommender.App_Start.MongoMigrations;

namespace MovieRecommender.App_Start
{
    public static class DataBaseConfiguration
    {
        /// <summary>
        /// Applies databese migrations defined in App_Start Migrations on DBS
        /// </summary>
        public static void ApplyMigrations()
        {
            MongoDbConnectionPool dbPool = UnityConfig.GetConfiguredContainer().Resolve<MongoDbConnectionPool>();
            var runner = new MigrationRunner(dbPool.Database);
            runner.MigrationLocator.LookForMigrationsInAssemblyOfType<Migration1>();
            runner.UpdateToLatest();
            runner.DatabaseStatus.ThrowIfNotLatestVersion();
        }
    }
}