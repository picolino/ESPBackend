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
using Shared;
using Shared.Logging;
using Microsoft.AspNet.Identity;
using OtpSharp;

namespace Authorization.Controllers
{
    [Authorize(Roles = Roles.User)]
    [RoutePrefix("manyfactor")]
    public class FactorController : ApiController
    {
        private readonly AuthRepository repository;
        private const string CurrentClassName = nameof(FactorController);
        private const string QrCodeImageGeneratorUrlPrefix = "http://qrcode.kaywa.com/img.php?s=4&d=";
        private const string IssuerName = "ESPB";
        private ILogger Logger => LoggerFactory.CreateLogger();

        public FactorController()
        {
            repository = new AuthRepository();
        }

        [HttpGet]
        [Route("google")]
        public async Task<IHttpActionResult> GoogleAuthEnable()
        {
            var secretKey = KeyGeneration.GenerateRandomKey(20);

            var userName = User.Identity.GetUserName();
            var barcodeUrl = KeyUrl.GetTotpUrl(secretKey, userName) + $"&issuer={IssuerName}";

            Logger.InfoWithIp(CurrentClassName, nameof(GoogleAuthEnable), $"Google auth enable request for user {userName}");

            var model = new GoogleAuthModel
                        {
                            Barcode = QrCodeImageGeneratorUrlPrefix + HttpUtility.UrlEncode(barcodeUrl),
                            SecretKey = Base32Encoder.Encode(secretKey)
                        };

            return Ok(model);
        }

        [HttpPut]
        [Route("google")]
        public async Task<IHttpActionResult> GoogleAuthConfirm(GoogleAuthModel model)
        {
            var secretKey = Base32Encoder.Decode(model.SecretKey);

            Logger.InfoWithIp(CurrentClassName, nameof(GoogleAuthEnable), $"Google auth confirm request for user {User.Identity.GetUserName()}");

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
            return BadRequest("The Code is not valid");
        }

        [HttpDelete]
        [Route("google")]
        public async Task<IHttpActionResult> GoogleAuthDelete()
        {
            Logger.InfoWithIp(CurrentClassName, nameof(GoogleAuthEnable), $"Google auth delete request for user {User.Identity.GetUserName()}");

            var user = await repository.FindById(User.Identity.GetUserId());
            user.IsGoogleAuthenticatorEnabled = false;
            user.GoogleAuthenticatorSecretKey = null;
            await repository.UpdateUser(user);

            return Ok();
        }
    }
}