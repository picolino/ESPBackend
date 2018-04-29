#region Usings

using System.Web.Http;
using ESPBackend.Application;
using Shared.Logging;
using ESPBackend.Dto;

#endregion

namespace ESPBackend.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/v1/rsa")]
    public class CryptoController : ApiController
    {
        private const string CurrentClassName = nameof(CryptoController);

        private readonly RsaCryptoService rsaCryptoService = new RsaCryptoService();

        private ILogger Logger => LoggerFactory.CreateLogger();

        [HttpGet]
        [Route("generate")]
        public IHttpActionResult GenerateKeyPair()
        {
            Logger.InfoWithIp(CurrentClassName, nameof(GenerateKeyPair), "RSA KeyPair generate request");

            var response = rsaCryptoService.GenerateRsaKeyPair();

            Logger.Debug(CurrentClassName, nameof(GenerateKeyPair), $"Public key: {response}");

            return Ok(response);
        }
    }
}