using AspNet.Identity.MongoDB;
using MongoDB.Driver;
using MovieRecommender.Database.Models;
using MovieRecommender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Database
{
    public class RepositoryManager : IRepositoryManager
    {
        private IMongoDatabase _database;

        private IMongoCollection<ApplicationUser> _users;
        private IMongoCollection<IdentityRole> _roles;
        private IMongoCollection<Movie> _movies;
        private IMongoCollection<MovieReview> _reviews;

        /// <summary>
        /// Main repository constructor
        /// </summary>
        /// <param name="pool"> injected by unity IoC to controllers</param>
        public RepositoryManager(MongoDbConnectionPool pool)
        {
            _database = pool.Database;
        }

        public IMongoCollection<ApplicationUser> Users
        {
            get
            {
                if (_users == null)
                {
                    _users = _database.GetCollection<ApplicationUser>("users");
                }
                return _users;
            }
        }

        public IMongoCollection<IdentityRole> Roles
        {
            get
            {
                if (_roles == null)
                {
                    _roles = _database.GetCollection<IdentityRole>("roles");
                }
                return _roles;
            }
        }

        public IMongoCollection<Movie> Movies
        {
            get
            {
                if (_movies == null)
                {
                    _movies = _database.GetCollection<Movie>("movies");
                }
                return _movies;
            }
        }

        public IMongoCollection<MovieReview> Reviews
        {
            get
            {
                if (_reviews == null)
                {
                    _reviews = _database.GetCollection<MovieReview>("reviews");
                }
                return _reviews;
            }
        }
    }
}