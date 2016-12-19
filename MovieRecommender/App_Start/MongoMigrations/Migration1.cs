using MongoMigrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.App_Start.MongoMigrations
{
    public class Migration1 : CollectionMigration
    {
        public Migration1()
            : base("0.0.1", "users")
        {
            Description = "Dummy migration.";
        }

        public override void ValidateMigrationDocuments()
        {
            // no validation needed
        }
    }
}