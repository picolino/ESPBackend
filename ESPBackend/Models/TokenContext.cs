using System.Linq;
using System.Net.Http;
using System.Security.Claims;

namespace ESPBackend.Models
{
    public static class TokenContext
    {
        public static string GetUserId(HttpRequestMessage request)
        {
            var context = request.GetOwinContext();
            return context.Authentication.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}