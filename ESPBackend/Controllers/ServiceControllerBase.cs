using System.Net;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;
using ESPBackend.Application;

namespace ESPBackend.Controllers
{
    public class ServiceControllerBase : ApiController
    {
        private void CreateDataContextFactory()
        {
            try
            {
                const string server = ".";
                const string database = "ESPB";
                const string sqlLogin = "espbworker";
                const string sqlPassword = "espbworker";

                RepositoryFactory.CreateDataContextFactory(server, database, sqlLogin, sqlPassword);
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
        }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            CreateDataContextFactory();
        }
    }
}