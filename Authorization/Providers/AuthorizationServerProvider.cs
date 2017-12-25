#region Usings

using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.OAuth;

#endregion

namespace Authorization.Providers
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var identityUser = new IdentityUser();

            using (var repository = new AuthRepository())
            {
                if (string.IsNullOrEmpty(context.Password))
                {
                    identityUser = await repository.FindEsp(context.UserName);

                    if (identityUser == null)
                    {
                        context.SetError("invalid_grant", "The user name is incorrect.");
                        return;
                    }
                }
                else
                {
                    identityUser = await repository.FindUser(context.UserName, context.Password);

                    if (identityUser == null)
                    {
                        context.SetError("invalid_grant", "The user name or password is incorrect.");
                        return;
                    }
                }
                
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, identityUser.Id));

            context.Validated(identity);
        }

        //public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        //{
            
        //}
    }
}