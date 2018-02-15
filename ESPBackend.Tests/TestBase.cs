using ESPBackend.DataAccessLayer.Interfaces;
using ESPBackend.Tests.Stubs;
using NUnit.Framework;

namespace ESPBackend.Tests
{
    public class TestBase
    {
        protected ITestDataRepository TestDataRepository;

        [SetUp]
        protected virtual void Setup()
        {
            TestDataRepository = new TestDataRepositoryStub();
        }
    }
}