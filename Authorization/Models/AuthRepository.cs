using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Authorization.Models
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

            return result;
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            var user = await userManager.FindAsync(userName, password);

            return user;
        }

        public void Dispose()
        {
            dbContext.Dispose();
            userManager.Dispose();

        }
    }
}