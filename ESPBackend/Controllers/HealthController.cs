using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Common.Logging;
using ESPBackend.Dto;

namespace ESPBackend.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/v1/health")]
    public class HealthController : ApiController
    {
        private const string CurrentClassName = nameof(HealthController);

        private ILogger Logger => LoggerFactory.CreateLogger();

        [HttpGet]
        [Route("ping")]
        public IHttpActionResult Ping()
        {
            Logger.Info(CurrentClassName, nameof(Ping), "Ping request");

            var response = new PingResponseDto();

            var versionAttribute = Assembly.GetExecutingAssembly()
                                           .GetCustomAttributes(typeof(AssemblyFileVersionAttribute))
                                           .FirstOrDefault() as AssemblyFileVersionAttribute;
            if (versionAttribute != null)
            {
                response.ServiceVersion = versionAttribute.Version;
            }
            response.Uptime = DateTime.Now - Process.GetCurrentProcess().StartTime;

            Logger.Debug(CurrentClassName, nameof(Ping), $"Ping response: {response}");

            return Ok(response);
        }
    }
}
