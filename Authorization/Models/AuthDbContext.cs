using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using Authorization.Domain;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Authorization.Models
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext()
            : base("AuthDbContext")
        {
        }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>().ToTable("Users", "dbo");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles", "dbo");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UsersRoles", "dbo");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UsersLogins", "dbo");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UsersClaims", "dbo");
        }
    }
}