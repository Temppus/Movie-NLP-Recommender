using MovieRecommender.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.StartupHooks
{
    public class CheckDbVersionOnStartup : IRunOnApplicationStart
    {
        public void Start()
        {
            DataBaseConfiguration.ApplyMigrations();
        }
    }
}