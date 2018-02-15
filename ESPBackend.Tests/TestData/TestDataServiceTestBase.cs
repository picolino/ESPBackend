using ESPBackend.Application;

namespace ESPBackend.Tests.TestData
{
    public class TestDataServiceTestBase : TestBase
    {
        protected TestDataService TestDataService;

        protected override void Setup()
        {
            base.Setup();
            TestDataService = new TestDataService(TestDataRepository);
        }
    }
}