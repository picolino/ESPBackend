using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Authorization.Models;
using Authorization.Providers;
using Base32;
using Common;
using Common.Logging;
using Microsoft.AspNet.Identity;
using OtpSharp;

namespace Authorization.Controllers
{
    [Authorize(Roles = Roles.User)]
    [RoutePrefix("api/v1/manyfactor")]
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
        [Route("google/generatesc")]
        public async Task<IHttpActionResult> GoogleAuthGetModel()
        {
            var secretKey = KeyGeneration.GenerateRandomKey(20);

            var userName = User.Identity.GetUserName();
            var barcodeUrl = KeyUrl.GetTotpUrl(secretKey, userName) + "&issuer=ESPB";

            var model = new GoogleAuthModel
                        {
                            Barcode = HttpUtility.UrlEncode(barcodeUrl),
                            SecretKey = Base32Encoder.Encode(secretKey)
                        };

            return Ok(model);
        }

        [HttpPost]
        [Route("google/savesc")]
        public async Task<IHttpActionResult> GoogleAuthEnable(GoogleAuthModel model)
        {
            byte[] secretKey = Base32Encoder.Decode(model.SecretKey);

            long timeStepMatched = 0;
            var otp = new Totp(secretKey);

            if (otp.VerifyTotp(model.InputCode, out timeStepMatched))
            {
                var user = await repository.FindById(User.Identity.GetUserId());
                user.IsGoogleAuthenticatorEnabled = true;
                user.GoogleAuthenticatorSecretKey = model.SecretKey;
                await repository.UpdateUser(user);

                return Ok();
            }
            else
            {
                return BadRequest("The Code is not valid");
            }
        }
    }
}