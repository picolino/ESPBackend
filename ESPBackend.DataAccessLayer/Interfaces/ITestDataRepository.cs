using ESPBackend.Dto;

namespace ESPBackend.DataAccessLayer.Interfaces
{
    public interface ITestDataRepository
    {
        TestData GetTestDataById(int id);
        int SaveTestData(TestDataDto testData, string userId);
    }
}