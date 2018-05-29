using System;
using ESPBackend.Dto;

namespace ESPBackend.DataAccessLayer.Interfaces
{
    public interface IDataContext : IDisposable
    {
        TestData TestDataGet(int id);
        int TestDataSave(TestDataDto testData, string userId);
        string GetAesKeyForUser(string userId);
    }
}