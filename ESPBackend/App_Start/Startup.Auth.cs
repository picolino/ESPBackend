#region Usings

using Microsoft.Owin.Security.OAuth;
using Owin;

#endregion

namespace ESPBackend
{
    public class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseOAuthBearerTokens(new OAuthAuthorizationServerOptions());
        }
    }
}