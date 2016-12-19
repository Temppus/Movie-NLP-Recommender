using AspNet.Identity.MongoDB;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using MovieRecommender.Models;
using Microsoft.Practices.Unity;
using MovieRecommender.Database;
using MongoDB.Driver;

namespace MovieRecommender.App_Start.IdentityConfiguration
{
    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        private IMongoCollection<IdentityRole> _roles;

        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
            : base(roleStore)
        {
            _roles = UnityConfig.GetConfiguredContainer().Resolve<RepositoryManager>().Roles;
        }

        public override Task<IdentityResult> CreateAsync(IdentityRole role)
        {
            return base.CreateAsync(role);
        }

        public override Task<IdentityResult> DeleteAsync(IdentityRole role)
        {
            return base.DeleteAsync(role);
        }

        public override Task<IdentityRole> FindByIdAsync(string roleId)
        {
            return base.FindByIdAsync(roleId);
        }

        public override Task<IdentityRole> FindByNameAsync(string roleName)
        {
            return base.FindByNameAsync(roleName);
        }

        public override Task<bool> RoleExistsAsync(string roleName)
        {
            return base.RoleExistsAsync(roleName);
        }

        public override IQueryable<IdentityRole> Roles
        {
            get
            {
                return _roles.Find(r => true).ToList().AsQueryable();
            }
        }

        public override Task<IdentityResult> UpdateAsync(IdentityRole role)
        {
            return base.UpdateAsync(role);
        }
    }
}