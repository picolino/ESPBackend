using System;
using System.Threading.Tasks;
using Authorization.Domain;
using Authorization.Models;
using Authorization.Models.Register;
using Shared;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Authorization.Providers
{
    public class AuthRepository : IDisposable
    {
        private readonly AuthDbContext dbContext;
        private readonly UserManager<AppUser> userManager;

        private const string GoogleAuthenticatorName = "GoogleAuthenticator";

        public AuthRepository()
        {
            dbContext = new AuthDbContext();
            userManager = new UserManager<AppUser>(new UserStore<AppUser>(dbContext));

            userManager.RegisterTwoFactorProvider(GoogleAuthenticatorName, new GoogleAuthenticatorTokenProvider());
        }

        public async Task<bool> ValidateGoogleAuth(string token, string userId)
        {
            return await userManager.VerifyTwoFactorTokenAsync(userId, GoogleAuthenticatorName, token);
        }

        public async Task<IdentityResult> RegisterUser(RegisterUserModel userModel)
        {
            var user = new AppUser
            {
                                    UserName = userModel.UserName
                                };

            var result = await userManager.CreateAsync(user, userModel.Password);
            if (result.Succeeded)
            {
                result = await userManager.AddToRoleAsync(user.Id, Roles.User);
            }

            return result;
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
            var user = await userManager.FindAsync(userName, password);

            return user;
        }

        public async Task<AppUser> FindById(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            return user;
        }

        public async Task<AppUser> FindEsp(string espName)
        {
            var user = await userManager.FindByNameAsync(espName);

            return user;
        }

        public async Task<IdentityResult> UpdateUser(AppUser user)
        {
            var updated = await userManager.UpdateAsync(user);

            if (!updated.Succeeded)
            {
                throw new Exception();
            }

            return updated;
        }

        public void Dispose()
        {
            dbContext.Dispose();
            userManager.Dispose();

        }
    }
}