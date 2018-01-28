using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Authorization.Providers;
using Base32;
using Common.Logging;
using Microsoft.AspNet.Identity;
using OtpSharp;

namespace Authorization.Controllers
{
    [Authorize]
    public class FactorController : ApiController
    {
        private readonly AuthRepository repository;
        private const string CurrentClassName = nameof(FactorController);
        private ILogger Logger => LoggerFactory.CreateLogger();

        public FactorController()
        {
            repository = new AuthRepository();
        }

        [HttpPost]
        public async Task<IHttpActionResult> EnableGoogleAuthenticator()
        {
            var secretKey = KeyGeneration.GenerateRandomKey(20);

            var userName = User.Identity.GetUserName();
            var barcodeUrl = KeyUrl.GetTotpUrl(secretKey, userName) + "&issuer=espb";

            //var model = new GoogleAuthenticatorViewModel
            //{
            //    SecretKey = Base32Encoder.Encode(secretKey),
            //    BarcodeUrl = HttpUtility.UrlEncode(barcodeUrl)
            //};
            return Ok();
        }
    }
}