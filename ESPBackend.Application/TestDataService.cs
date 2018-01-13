using ESPBackend.DataAccessLayer;
using ESPBackend.DataAccessLayer.Interfaces;
using ESPBackend.Dto;

namespace ESPBackend.Application
{
    public class TestDataService
    {
        private readonly ITestDataRepository testDataRepository;

        public TestDataService(ITestDataRepository testDataRepository)
        {
            this.testDataRepository = testDataRepository;
        }

        public int Save(TestDataDto testData, string userId)
        {
            var id = testDataRepository.SaveTestData(testData, userId);
            return id;
        }

        public TestData GetBy(int id)
        {
            return testDataRepository.GetTestDataById(id);
        }
    }
}