using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Common.Logging;
using ESPBackend.Domain;
using ESPBackend.Dto;
using ESPBackend.Models;

namespace ESPBackend.Controllers
{
    [Authorize]
    [RoutePrefix("api/v1/repo")]
    public class RepositoryController : ApiController
    {
        private const string CurrentClassName = nameof(HealthController);
        private ILogger Logger => LoggerFactory.CreateLogger();

        [HttpPost]
        [Route("savetestdata")]
        public IHttpActionResult SaveData(TestDataDto data)
        {
            Logger.Info(CurrentClassName, nameof(SaveData), $"SaveData request with {data}");

            var insertedId = -1;

            using (var dbContext = new ESPBDbContext())
            {
                var testData = new TestData
                {
                    TestString = data.TestString,
                    UserId = TokenContext.GetUserId(Request)
                };
                dbContext.TestData.Add(testData);
                dbContext.SaveChanges();
                insertedId = testData.Id;
            }

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

            var data = new TestData();

            using (var dbContext = new ESPBDbContext())
            {
                data = dbContext.TestData.Find(dataId);
            }

            if (data is null)
            {
                return NotFound();
            }

            return Ok(data);
        }
    }
}