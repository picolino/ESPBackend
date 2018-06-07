using System.Threading.Tasks;
using System.Web.Http;
using Authorization.Models;
using Authorization.Models.Register;
using Authorization.Providers;
using Shared.Logging;
using Microsoft.AspNet.Identity;

namespace Authorization.Controllers
{
    public class AccountController : ApiController
    {
        private readonly AuthRepository repository;
        private const string CurrentClassName = nameof(AccountController);
        private ILogger Logger => LoggerFactory.CreateLogger();

        public AccountController()
        {
            repository = new AuthRepository(Logger);
        }
        
        [HttpPost]
        [AllowAnonymous]
        [Route("user/register")]
        public async Task<IHttpActionResult> RegisterUser(RegisterUserModel userModel)
        {
            Logger.InfoWithIp(CurrentClassName, nameof(RegisterUser), $"RegisterUser request with {userModel}");

            if (!ModelState.IsValid)
            {
                Logger.Debug(CurrentClassName, nameof(RegisterUser), $"Model state is not valid. Returning 'BadRequest'");
                return BadRequest(ModelState);
            }

            await repository.RegisterUser(userModel);
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("esp/register")]
        public async Task<IHttpActionResult> RegisterEsp(RegisterESPModel espModel)
        {
            Logger.InfoWithIp(CurrentClassName, nameof(RegisterEsp), $"RegisterEsp request with {espModel}");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await repository.RegisterEsp(espModel);

            var errorResult = GetErrorResult(result);
            return errorResult ?? Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                repository.Dispose();
            }

            base.Dispose(disposing);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}