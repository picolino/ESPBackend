using System;
using System.Threading.Tasks;
using Authorization.Models;
using Authorization.Models.Register;
using Common;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Authorization.Providers
{
    public class AuthRepository : IDisposable
    {
        private readonly AuthDbContext dbContext;
        private readonly UserManager<IdentityUser> userManager;

        public AuthRepository()
        {
            dbContext = new AuthDbContext();
            userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(dbContext));
        }

        public async Task<IdentityResult> RegisterUser(RegisterUserModel userModel)
        {
            var user = new IdentityUser
                                {
                                    UserName = userModel.UserName
                                };

            var result = await userManager.CreateAsync(user, userModel.Password);
            result = await userManager.AddToRoleAsync(user.Id, Roles.User);

            return result;
        }

        public async Task<IdentityResult> RegisterEsp(RegisterESPModel espModel)
        {
            userManager.UserValidator = new UserValidator<IdentityUser>(userManager)
                                        {
                                            AllowOnlyAlphanumericUserNames = false
                                        };
            var esp = new IdentityUser
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

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            var user = await userManager.FindAsync(userName, password);

            return user;
        }

        public async Task<IdentityUser> FindEsp(string espName)
        {
            var user = await userManager.FindByNameAsync(espName);

            return user;
        }

        public void Dispose()
        {
            dbContext.Dispose();
            userManager.Dispose();

        }
    }
}