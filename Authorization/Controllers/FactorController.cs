using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;
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
            repository = new AuthRepository(Logger);
        }

        #region Google

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
            Logger.Debug(CurrentClassName, nameof(GoogleAuthConfirm), $"Decoding secret key '{model.SecretKey}'");
            var secretKey = Base32Encoder.Decode(model.SecretKey);

            Logger.InfoWithIp(CurrentClassName, nameof(GoogleAuthConfirm), $"Google auth confirm request for user {User.Identity.GetUserName()}");

            long timeStepMatched = 0;
            Logger.Debug(CurrentClassName, nameof(GoogleAuthConfirm), $"Generating TOTP-key");
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

        #endregion

        #region Email

        [HttpPost]
        [Route("email")]
        public async Task<IHttpActionResult> SaveEmail(EmailModel email)
        {
            Logger.InfoWithIp(CurrentClassName, nameof(SaveEmail), $"Saving emai requestl '{email.Email}'");
            if (!email.Email.Contains("@") || !email.Email.Contains(".") || email.Email.Length < 3)
            {
                return BadRequest("Incorrect email format");
            }
            
            var isEmailSaved = await repository.SaveEmail(email.Email, User.Identity.GetUserId());
            if (isEmailSaved)
            {
                return Ok();
            }

            return InternalServerError();
        }

        [HttpGet]
        [Route("email/confirm")]
        public async Task<IHttpActionResult> SendConfirmation()
        {
            var userId = User.Identity.GetUserId();
            Logger.InfoWithIp(CurrentClassName, nameof(SendConfirmation), $"Send confirmation to email request for user id '{userId}'");
            var isEmailAlreadyConfirmed = await repository.IsEmailConfirmed(userId);
            if (isEmailAlreadyConfirmed)
            {
                return Ok("Email is already confirmed");
            }
            
            var email = await repository.GetEmailByUserId(userId);
            var token = await repository.GetEmailConfirmationToken(userId);

            Logger.Debug(CurrentClassName, nameof(SendConfirmation), $"Generating url confirmation link...");
            var callbackUrl = Url.Link("GetConfirmationRoute", new { userId, token });

            var emailProvider = new EmailProvider(Logger);

            await emailProvider.SendAsync(email, "Email Confirmation", $"For email confirmation go to the link: {callbackUrl}");

            return Ok("Confirmation email was sended");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("email/getconfirm", Name = "GetConfirmationRoute")]
        public async Task<IHttpActionResult> GetConfirmation(string userId = "", string token = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("User Id and Token are required");
            }

            var succeed = await repository.ConfirmEmail(userId, token);

            if (succeed)
            {
                return Ok("Ваш e-mail успешно подтвержден");
            }
            else
            {
                return BadRequest("Wrong token. Retry the operation.");
            }
        }

        #endregion


    }
}