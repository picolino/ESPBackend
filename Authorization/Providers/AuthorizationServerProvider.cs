#region Usings

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Authorization.Domain;
using Shared;
using Shared.Logging;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
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
            string clientId;
            using (var repository = new AuthRepository())
            {
                AppUser user;
                var grantType = context.Parameters.Get("grant_type");
                switch (grantType)
                {
                    case "password":
                        user = await repository.FindUser(context.Parameters.Get("UserName"), context.Parameters.Get("Password"));

                        if (user == null)
                        {
                            context.SetError("invalid_grant", "The user name or password is incorrect.");
                            return;
                        }

                        clientId = user.Id;

                        if (user.IsGoogleAuthenticatorEnabled)
                        {
                            var totp = context.Parameters.Get("totp");
                            if (totp == null)
                            {
                                context.SetError("invalid_grant", "Set TOTP code");
                                return;
                            }
                            
                            var validated = await repository.ValidateGoogleAuth(totp, user.Id);
                            if (!validated)
                            {
                                context.SetError("invalid_grant", "TOTP code is incorrect");
                                return;
                            }
                        }

                        break;
                    case "esp":
                        var espIdentifier = context.Parameters.Get("espid");
                        if (espIdentifier != null)
                        {
                            user = await repository.FindEsp(espIdentifier);

                            if (user == null)
                            {
                                context.SetError("invalid_grant", "The esp identifier is incorrect.");
                                return;
                            }

                            clientId = user.Id;
                        }
                        else
                        {
                            context.SetError("invalid_grant");
                            return;
                        }

                        break;
                    default:
                        context.SetError("Bad grant_type");
                        return;
                }
            }

            context.Validated(clientId);
        }

        //ВЫДАЧА ТОКЕНА ДЛЯ ПОЛЬЗОВАТЕЛЯ
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //Да-да, записываем пароль в логи, все ок, логи закрыты извне, необходимо для простой отладки в случае чего =)
            Logger.InfoWithIp(CurrentClassName, nameof(GrantResourceOwnerCredentials), $"Token request with username: {context.UserName} and password {context.Password}");

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, Roles.User));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, context.ClientId));

            context.Validated(identity);

            Logger.Debug(CurrentClassName, nameof(GrantResourceOwnerCredentials), $"Successful token with username: {context.UserName} (Id - {context.ClientId})");
        }

        //ВЫДАЧА ТОКЕНА ДЛЯ ESP
        public override async Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var espIdentifier = context.Parameters.Get("espid");

            Logger.InfoWithIp(CurrentClassName, nameof(GrantCustomExtension), $"Token request with ESP Identifier: {espIdentifier}");

            Logger.Debug(CurrentClassName, nameof(GrantCustomExtension), $"Successful token with ESP Identifier: {espIdentifier} (Id - {context.ClientId}");

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, espIdentifier));
            identity.AddClaim(new Claim(ClaimTypes.Role, Roles.Esp));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, context.ClientId));

            context.Validated(identity);
        }

        //Добавление в токен кастомных значений
        //public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        //{

        //}
    }
}