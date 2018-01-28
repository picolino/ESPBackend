using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Authorization.Domain
{
    public class AppUser : IdentityUser
    {
        public bool IsGoogleAuthenticatorEnabled { get; set; }

        public string GoogleAuthenticatorSecretKey { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ExternalBearer);
            return userIdentity;
        }
    }
}