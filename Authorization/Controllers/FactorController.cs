#region Usings

using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Authorization.Models;
using Authorization.Providers;
using Authorization.Services.Modules;
using Base32;
using Microsoft.AspNet.Identity;
using OtpSharp;
using Shared;
using Shared.Logging;

#endregion

namespace Authorization.Controllers
{
    [Authorize(Roles = Roles.User)]
    [RoutePrefix("manyfactor")]
    public class FactorController : ApiController
    {
        private const string CurrentClassName = nameof(FactorController);
        private const string IssuerName = "ESPB";
        private readonly AuthRepository repository;

        private readonly GoogleAuthenticatorModule googleAuthModule;

        public FactorController()
        {
            repository = new AuthRepository(Logger);
            googleAuthModule = new GoogleAuthenticatorModule(Logger, repository, IssuerName);
        }

        private ILogger Logger => LoggerFactory.CreateLogger();

        #region GoogleAuthenticator

        [HttpPut]
        [Route("google")]
        public async Task<IHttpActionResult> GoogleAuthConfirm(GoogleAuthConfirmationModel confirmationModel)
        {
            Logger.InfoWithIp(CurrentClassName, nameof(GoogleAuthConfirm), $"Google auth confirm request for user {User.Identity.GetUserName()}");
            await googleAuthModule.Confirm(confirmationModel, User.Identity.GetUserId());
            return Ok();
        }

        [HttpDelete]
        [Route("google")]
        public async Task<IHttpActionResult> GoogleAuthDelete()
        {
            Logger.InfoWithIp(CurrentClassName, nameof(GoogleAuthEnable), $"Google auth delete request for user {User.Identity.GetUserName()}");
            await googleAuthModule.Disable(User.Identity.GetUserId());
            return Ok();
        }

        [HttpGet]
        [Route("google")]
        public async Task<IHttpActionResult> GoogleAuthEnable()
        {
            var userName = User.Identity.GetUserName();
            Logger.InfoWithIp(CurrentClassName, nameof(GoogleAuthConfirm), $"Google auth confirm request for user {userName}");
            var secrets = await googleAuthModule.GetSecret(userName);
            return Ok(secrets);
        }

        #endregion

        #region Email

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

            return BadRequest("Wrong token. Retry the operation.");
        }

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

        #endregion
    }
}