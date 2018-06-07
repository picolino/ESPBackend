using System;
using System.Linq;
using System.Threading.Tasks;
using Authorization.Domain;
using Authorization.Models;
using Authorization.Models.Register;
using Shared;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Shared.Logging;

namespace Authorization.Providers
{
    public class AuthRepository : IDisposable
    {
        private const string CurrentClassName = nameof(AuthRepository);

        private readonly AuthDbContext dbContext;
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger logger;

        private const string GoogleAuthenticatorName = "GoogleAuthenticator";

        public AuthRepository(ILogger logger)
        {
            this.logger = logger;

            dbContext = new AuthDbContext();
            userManager = new UserManager<AppUser>(new UserStore<AppUser>(dbContext));

            userManager.RegisterTwoFactorProvider(GoogleAuthenticatorName, new GoogleAuthenticatorTokenProvider());
            userManager.UserTokenProvider = new EmailTokenProvider<AppUser>();
        }

        public async Task<bool> SaveEmail(string email, string userId)
        {
            logger.Debug(CurrentClassName, nameof(SaveEmail), $"Setting email '{email}' for user with id '{userId}'");
            var result = await userManager.SetEmailAsync(userId, email);
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                throw new IdentityResultException("An error has occured with saving email", result.Errors);
            }
        }

        public async Task<string> GetEmailByUserId(string userId)
        {
            logger.Debug(CurrentClassName, nameof(GetEmailByUserId), $"Getting email for user with id '{userId}'");
            var email = await userManager.GetEmailAsync(userId);
            return email;
        }

        public async Task<string> GetEmailConfirmationToken(string userId)
        {
            logger.Debug(CurrentClassName, nameof(GetEmailConfirmationToken), $"Getting email confirmation token for user with id '{userId}'");
            var token = await userManager.GenerateEmailConfirmationTokenAsync(userId);
            return token;
        }

        public async Task<bool> ConfirmEmail(string userId, string token)
        {
            logger.Debug(CurrentClassName, nameof(ConfirmEmail), $"Confirmation email for user with id '{userId}'");
            var result = await userManager.ConfirmEmailAsync(userId, token);
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                throw new IdentityResultException("An error has occured with email confirmation", result.Errors);
            }
        }

        public async Task<bool> IsEmailConfirmed(string userId)
        {
            logger.Debug(CurrentClassName, nameof(IsEmailConfirmed), $"Checking email confirmation for user with id '{userId}'");
            var result = await userManager.IsEmailConfirmedAsync(userId);
            return result;
        }

        public async Task<bool> ValidateGoogleAuth(string token, string userId)
        {
            logger.Debug(CurrentClassName, nameof(ValidateGoogleAuth), $"Validating google auth for user with id '{userId}'");
            return await userManager.VerifyTwoFactorTokenAsync(userId, GoogleAuthenticatorName, token);
        }

        public async Task<IdentityResult> RegisterUser(RegisterUserModel userModel)
        {
            logger.Debug(CurrentClassName, nameof(RegisterUser), $"Creating user object with login: '{userModel.UserName}', password matching: '{userModel.ConfirmPassword == userModel.Password}'");
            var user = new AppUser
                       {
                           UserName = userModel.UserName
                       };

            logger.Debug(CurrentClassName, nameof(RegisterUser), $"Creating user '{user.UserName}' in database by user object '{userModel.UserName}'");
            var result = await userManager.CreateAsync(user, userModel.Password);
            if (result.Succeeded)
            {
                logger.Debug(CurrentClassName, nameof(RegisterUser), $"User successfully created. Adding user role to created user '{user.UserName}'");
                result = await userManager.AddToRoleAsync(user.Id, Roles.User);
                if (result.Succeeded)
                {
                    return result;
                }
                else
                {
                    throw new IdentityResultException("An error has occured with adding role", result.Errors);
                }
            }
            else
            {
                throw new IdentityResultException("An error has occured with registration", result.Errors);
            }
        }

        public async Task<IdentityResult> RegisterEsp(RegisterESPModel espModel)
        {
            userManager.UserValidator = new UserValidator<AppUser>(userManager)
                                        {
                                            AllowOnlyAlphanumericUserNames = false
                                        };
            var esp = new AppUser
                       {
                           UserName = espModel.ESPIdentifier
                       };

            var result = await userManager.CreateAsync(esp);
            if (result.Succeeded)
            {
                result = await userManager.AddToRoleAsync(esp.Id, Roles.Esp);
            }

            return result;
        }

        public async Task<AppUser> FindUser(string userName, string password)
        {
            logger.Debug(CurrentClassName, nameof(FindUser), $"Finding user with username '{userName}'");
            var user = await userManager.FindAsync(userName, password);
            return user;
        }

        public async Task<AppUser> FindById(string userId)
        {
            logger.Debug(CurrentClassName, nameof(FindById), $"Finding user with user id '{userId}'");
            var user = await userManager.FindByIdAsync(userId);
            return user;
        }

        public async Task<AppUser> FindEsp(string espName)
        {
            var user = await userManager.FindByNameAsync(espName);

            return user;
        }

        public async Task<bool> UpdateUser(AppUser user)
        {
            logger.Debug(CurrentClassName, nameof(UpdateUser), $"Updating user");
            var updated = await userManager.UpdateAsync(user);
            
            if (updated.Succeeded)
            {
                return true;
            }
            else
            {
                throw new IdentityResultException("An error has occured with user updating", updated.Errors);
            }
        }

        public void Dispose()
        {
            dbContext.Dispose();
            userManager.Dispose();

        }
    }
}