using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Shared;
using Shared.Logging;
using ESPBackend.Application;
using ESPBackend.Domain;
using ESPBackend.Dto;
using Microsoft.AspNet.Identity;

namespace ESPBackend.Controllers
{
    [Authorize(Roles = Roles.Esp)]
    [RoutePrefix("api/v1/testdata")]
    public class RepositoryController : ServiceControllerBase
    {
        private const string CurrentClassName = nameof(RepositoryController);
        private ILogger Logger => LoggerFactory.CreateLogger();
        private TestDataService TestDataService => new TestDataService(RepositoryFactory.TestDataRepository);
        private AesCryptoService AesCryptoService => new AesCryptoService(RepositoryFactory.CryptoRepository);

        [Route]
        [HttpPut]
        public IHttpActionResult SaveData(AesEncryptedTestDataDto testData)
        {
            try
            {
                Logger.InfoWithIp(CurrentClassName, nameof(SaveData), $"SaveData request with {testData}");

                if (testData == null)
                {
                    return BadRequest("Something bad. Check your request.");
                }

                if (testData.IsEncryptedData)
                {
                    testData.TestString = AesCryptoService.Decrypt(User.Identity.GetUserId(), testData.AesEncryptedData);
                }

                if (string.IsNullOrEmpty(testData.TestString))
                {
                    return BadRequest("Something bad. Check your request.");
                }

                var insertedId = TestDataService.Save(testData, User.Identity.GetUserId());

                if (insertedId <= 0)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occured with data saving"));
                }

                return Ok($"TestData was added. TestData Id: {insertedId}");
            }
            catch (Exception e)
            {
                Logger.Error(CurrentClassName, nameof(SaveData), e);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e));
            }
        }

        [Route]
        [HttpGet]
        public IHttpActionResult GetData(int id)
        {
            try
            {
                Logger.InfoWithIp(CurrentClassName, nameof(GetData), $"GetData request with ID = {id}");

                var testData = TestDataService.GetBy(id);

                if (testData is null)
                {
                    var resultMsg = $"Cant find test data with ID = {id}";
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, resultMsg));
                }

                var result = new TestDataDomain
                             {
                                 Id = testData.Id,
                                 TestData = testData.TestString,
                                 UserId = testData.UserId
                             };

                return Ok(result);
            }
            catch (Exception e)
            {
                Logger.Error(CurrentClassName, nameof(SaveData), e);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e));
            }
            
        }
    }
}