using Authorization.Domain;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Authorization.Models
{
    public class AuthDbContext : IdentityDbContext<UserModel>
    {
        public AuthDbContext()
            : base("AuthDbContext")
        {
        }

        public static AuthDbContext Create()
        {
            return new AuthDbContext();
        }


    }
}