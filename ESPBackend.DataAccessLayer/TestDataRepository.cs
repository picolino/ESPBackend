using ESPBackend.DataAccessLayer.Interfaces;
using ESPBackend.Dto;

namespace ESPBackend.DataAccessLayer
{
    public class TestDataRepository : ITestDataRepository
    {
        private readonly IDataContextFactory dataContextFactory;

        public TestDataRepository(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        public TestData GetTestDataById(int id)
        {
            TestData testData;

            using (var dataContext = dataContextFactory.Create())
            {
                testData = dataContext.TestDataGet(id);
            }

            return testData;
        }

        public int SaveTestData(TestDataDto testData, string userId)
        {
            var id = -1;
            using (var dataContext = dataContextFactory.Create())
            {
                id = dataContext.TestDataSave(testData, userId);
            }
            return id;
        }
    }
}