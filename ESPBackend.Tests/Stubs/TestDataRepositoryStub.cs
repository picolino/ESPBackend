#region Usings

using System.Collections.Generic;
using ESPBackend.DataAccessLayer.Interfaces;
using ESPBackend.Dto;

#endregion

namespace ESPBackend.Tests.Stubs
{
    public class TestDataRepositoryStub : ITestDataRepository
    {
        private readonly List<DataAccessLayer.TestData> testDataStorage;

        public TestDataRepositoryStub()
        {
            testDataStorage = new List<DataAccessLayer.TestData>();
        }

        public DataAccessLayer.TestData GetTestDataById(int id)
        {
            return testDataStorage[id];
        }

        public int SaveTestData(TestDataDto testData, string userId)
        {
            var testDataForSave = new DataAccessLayer.TestData
                                  {
                                      Id = testDataStorage.Count,
                                      TestString = testData.TestString,
                                      UserId = userId
                                  };
            testDataStorage.Add(testDataForSave);
            return testDataForSave.Id;
        }
    }
}