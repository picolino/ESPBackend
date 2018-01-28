#region Usings

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Authorization.Domain;
using Common;
using Common.Logging;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.OAuth;

#endregion

namespace Authorization.Providers
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private const string CurrentClassName = nameof(AuthorizationServerProvider);
        private ILogger Logger => LoggerFactory.CreateLogger();

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        //ВЫДАЧА ТОКЕНА ДЛЯ ПОЛЬЗОВАТЕЛЯ
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            Logger.Info(CurrentClassName, nameof(GrantResourceOwnerCredentials), $"Token request with username: {context.UserName} and password");

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var appUser = new AppUser();

            using (var repository = new AuthRepository())
            {
                appUser = await repository.FindUser(context.UserName, context.Password);

                if (appUser == null)
                {

                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
            }
            
            Logger.Debug(CurrentClassName, nameof(GrantResourceOwnerCredentials), $"Successful token with username: {context.UserName} (Id - {appUser.Id})");

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, Roles.User));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, appUser.Id));

            context.Validated(identity);
        }

        //ВЫДАЧА ТОКЕНА ДЛЯ ESP
        public override async Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)
        {

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var espIdentifier = context.Parameters.Get("espid");
            
            Logger.Info(CurrentClassName, nameof(GrantCustomExtension), $"Token request with ESP Identifier: {espIdentifier}");

            var appEsp = new AppUser();

            using (var repository = new AuthRepository())
            {
                if (context.GrantType == "esp" && espIdentifier != null)
                {
                    appEsp = await repository.FindEsp(espIdentifier);

                    if (appEsp == null)
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

            Logger.Debug(CurrentClassName, nameof(GrantCustomExtension), $"Successful token with ESP Identifier: {espIdentifier} (Id - {appEsp.Id}");

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, espIdentifier));
            identity.AddClaim(new Claim(ClaimTypes.Role, Roles.Esp));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, appEsp.Id));

            context.Validated(identity);
        }

        //public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        //{
            
        //}
    }
}