#region Usings

using System.Security.Claims;
using System.Threading.Tasks;
using Common;
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
                identityUser = await repository.FindUser(context.UserName, context.Password);

                if (identityUser == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, Roles.User));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, identityUser.Id));

            context.Validated(identity);
        }

        public override async Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var espIdentifier = context.Parameters.Get("espid");
            var identityEsp = new IdentityUser();

            using (var repository = new AuthRepository())
            {
                if (context.GrantType == "esp" && espIdentifier != null)
                {
                    identityEsp = await repository.FindEsp(espIdentifier);

                    if (identityEsp == null)
                    {
                        context.SetError("invalid_grant", "The esp identifier is incorrect.");
                        return;
                    }
                }
                else
                {
                    context.SetError("invalid_grant");
                    return;
                }

            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, espIdentifier));
            identity.AddClaim(new Claim(ClaimTypes.Role, Roles.Esp));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, identityEsp.Id));

            context.Validated(identity);
        }

        //public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        //{
            
        //}
    }
}