#region Usings

using System;
using Authorization;
using Authorization.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

#endregion

[assembly: OwinStartup(typeof(Startup))]
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
                                                TokenEndpointPath = new PathString("/auth/token"),
                                                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                                                Provider = new AuthorizationServerProvider()
                                            });
            app.UseOAuthBearerTokens(new OAuthAuthorizationServerOptions());
        }
    }
}