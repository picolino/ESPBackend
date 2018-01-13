using System.Net;
using System.Web.Http;
using Common;
using Common.Logging;
using ESPBackend.Application;
using ESPBackend.Dto;
using ESPBackend.Models;

namespace ESPBackend.Controllers
{
    [Authorize(Roles = Roles.Esp)]
    [RoutePrefix("api/v1/repo")]
    public class RepositoryController : ServiceControllerBase
    {
        private const string CurrentClassName = nameof(HealthController);
        private ILogger Logger => LoggerFactory.CreateLogger();
        private TestDataService TestDataService => new TestDataService(RepositoryFactory.TestDataRepository);

        [HttpPost]
        [Route("savetestdata")]
        public IHttpActionResult SaveData(TestDataDto data)
        {
            Logger.Info(CurrentClassName, nameof(SaveData), $"SaveData request with {data}");

            var insertedId = TestDataService.Save(data, TokenContext.GetUserId(Request));

            if (insertedId <= 0)
            {
                return InternalServerError();
            }

            return Ok($"TestData was added. TestData Id: {insertedId}");
        }

        [HttpPost]
        [Route("gettestdata")]
        public IHttpActionResult GetData([FromBody] int dataId)
        {
            Logger.Info(CurrentClassName, nameof(GetData), $"GetData request with {dataId}");
            
            var testData = TestDataService.GetBy(dataId);

            if (testData is null)
            {
                return NotFound();
            }

            return Ok(testData);
        }
    }
}