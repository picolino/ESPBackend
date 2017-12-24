#region Usings

using ESPBackend;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

#endregion

[assembly: OwinStartup(typeof(Startup))]
namespace ESPBackend
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseOAuthBearerTokens(new OAuthAuthorizationServerOptions());
        }
    }
}