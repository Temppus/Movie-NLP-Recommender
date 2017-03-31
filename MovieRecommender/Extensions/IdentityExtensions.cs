using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace MovieRecommender.Extensions
{
    public static class IdentityExtensions
    {
        public static string IsExperimentDone(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity)?.FindFirst("ExperimentDone");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string IsExperimentInPogress(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity)?.FindFirst("ExperimentProgress");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}