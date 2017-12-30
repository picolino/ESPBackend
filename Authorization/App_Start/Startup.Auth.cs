#region Usings

using System;
using Authorization.Models;
using Authorization.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

#endregion

namespace Authorization
{
    public partial class Startup
    {
        private void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(() => new AuthDbContext());

            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
                                            {
                                                AllowInsecureHttp = true,
                                                TokenEndpointPath = new PathString("/token"),
                                                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                                                Provider = new AuthorizationServerProvider()
                                            });
            app.UseOAuthBearerTokens(new OAuthAuthorizationServerOptions());
        }
    }
}