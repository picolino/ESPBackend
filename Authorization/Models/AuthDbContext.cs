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


            modelBuilder.Entity<IdentityUser>().ToTable("Users", "dbo");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles", "dbo");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles", "dbo");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims", "dbo");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins", "dbo");

        }
    }
}