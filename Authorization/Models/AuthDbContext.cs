using Microsoft.AspNet.Identity.EntityFramework;

namespace Authorization.Models
{
    public class AuthDbContext : IdentityDbContext<IdentityUser>
    {
        public AuthDbContext()
            : base("AuthDbContext")
        {
        }
    }
}