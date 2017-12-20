using System;
using System.Web.Http;
using Authorization;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace Authorization
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            WebApiConfig.Register(config);
            ConfigureAuth(app);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
        }
    }
}