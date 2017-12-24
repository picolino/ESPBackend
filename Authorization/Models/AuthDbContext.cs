using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
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
            var user = modelBuilder.Entity<IdentityUser>().ToTable("Users", "dbo");
            user.Ignore(u => u.Roles);
            user.Ignore(u => u.Claims);
            user.Ignore(u => u.Logins);
            user.Ignore(u => u.LockoutEnabled);
            user.Ignore(u => u.LockoutEndDateUtc);
            user.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex") { IsUnique = true }));
            
            user.Property(u => u.Email).HasMaxLength(256);

            modelBuilder.Ignore<IdentityRole>();
            modelBuilder.Ignore<IdentityUserRole>();
            modelBuilder.Ignore<IdentityUserLogin>();
            modelBuilder.Ignore<IdentityUserClaim>();

        }
    }
}