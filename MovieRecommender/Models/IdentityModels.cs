using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using AspNet.Identity.MongoDB;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using System;
using MongoDB.Bson;

namespace MovieRecommender.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public IEnumerable<MovieLikeInfo> LikedMovies { get; set; } = new List<MovieLikeInfo>();

        public IEnumerable<MovieLikeInfo> NotInterestedMovies { get; set; } = new List<MovieLikeInfo>();

        public bool ExperimentDone { get; set; } = false;

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            userIdentity.AddClaim(new Claim("ExperimentDone", ExperimentDone ? "True" : "False"));

            return userIdentity;
        }
    }

    public class MovieLikeInfo
    {
        [BsonId]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public string IMDBId { get; set; }
    }
}