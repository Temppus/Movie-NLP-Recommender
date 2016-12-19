﻿using AspNet.Identity.MongoDB;
using MongoDB.Driver;
using MovieRecommender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Database
{
    public interface IRepositoryManager
    {
        IMongoCollection<ApplicationUser> Users { get; }
        IMongoCollection<IdentityRole> Roles { get; }
    }
}