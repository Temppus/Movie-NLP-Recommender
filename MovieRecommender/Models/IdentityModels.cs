using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using AspNet.Identity.MongoDB;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using System;
using MongoDB.Bson;
using System.Linq;

namespace MovieRecommender.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    [BsonIgnoreExtraElements]
    public class ApplicationUser : IdentityUser
    {
        public IEnumerable<MovieLikeInfo> LikedMovies { get; set; } = new List<MovieLikeInfo>();

        public IEnumerable<MovieLikeInfo> NotInterestedMovies { get; set; } = new List<MovieLikeInfo>();
            
        public Experiment ExperimentResult { get; set; } = null;

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            int experimentRatedCount = 0;

            if (ExperimentResult != null)
            {
                experimentRatedCount = ExperimentResult.WouldWatchIds().Count() + ExperimentResult.WouldNotWatchIds().Count();
            }

            userIdentity.AddClaim(new Claim("ExperimentDone", experimentRatedCount >= 15 ? "True" : "False"));
            userIdentity.AddClaim(new Claim("ExperimentProgress", experimentRatedCount > 0 ? "True" : "False"));

            return userIdentity;
        }
    }

    public class Experiment
    {
        public IEnumerable<MovieChoice> WatchedChoice { get; set; } = new List<MovieChoice>();
        public IEnumerable<MovieChoice> WouldWatchChoice { get; set; } = new List<MovieChoice>();
        public IEnumerable<MovieChoice> WouldNotWatchChoice { get; set; } = new List<MovieChoice>();

        public IEnumerable<string> WatchedIds()
        {
            return WatchedChoice.Select(x => x.IMDBID);
        }

        public IEnumerable<string> WouldWatchIds()
        {
            return WouldWatchChoice.Select(x => x.IMDBID);
        }

        public IEnumerable<string> WouldNotWatchIds()
        {
            return WouldNotWatchChoice.Select(x => x.IMDBID);
        }
    }

    public class MovieLikeInfo
    {
        [BsonId]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public string IMDBId { get; set; }
    }
}