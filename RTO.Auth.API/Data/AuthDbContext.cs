using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RTO.Auth.API.Entities;
using System.Reflection;

namespace RTO.Auth.API.Data
{
    public class AuthDbContext : IdentityDbContext<UserModel>
    {

        public AuthDbContext()
        {

        }

        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
