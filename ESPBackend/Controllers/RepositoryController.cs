using System.Net;
using System.Net.Http;
using System.Web.Http;
using Shared;
using Shared.Logging;
using ESPBackend.Application;
using ESPBackend.Dto;
using Microsoft.AspNet.Identity;

namespace ESPBackend.Controllers
{
    [Authorize(Roles = Roles.Esp)]
    [RoutePrefix("api/v1/testdata")]
    public class RepositoryController : ServiceControllerBase
    {
        private const string CurrentClassName = nameof(HealthController);
        private ILogger Logger => LoggerFactory.CreateLogger();
        private TestDataService TestDataService => new TestDataService(RepositoryFactory.TestDataRepository);

        [Route]
        [HttpPut]
        public IHttpActionResult SaveData(TestDataDto data)
        {
            Logger.InfoWithIp(CurrentClassName, nameof(SaveData), $"SaveData request with {data}");
            
            var insertedId = TestDataService.Save(data, User.Identity.GetUserId());

            if (insertedId <= 0)
            {
                return InternalServerError();
            }

            return Ok($"TestData was added. TestData Id: {insertedId}");
        }

        [Route]
        [HttpGet]
        public IHttpActionResult GetData(int id)
        {
            Logger.InfoWithIp(CurrentClassName, nameof(GetData), $"GetData request with {id}");
            
            var testData = TestDataService.GetBy(id);

            if (testData is null)
            {
                return Content(HttpStatusCode.NotFound, $"Cant find test data with ID = {id}");
            }

            return Ok(testData);
        }
    }
}