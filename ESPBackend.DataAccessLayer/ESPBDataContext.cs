using System;
using System.Data.Entity;
using ESPBackend.DataAccessLayer.Interfaces;
using ESPBackend.Dto;

namespace ESPBackend.DataAccessLayer
{
    public partial class Entities : IDataContext
    {
        public Entities(string connectionString) : base(connectionString)
        {
        }

        public TestData TestDataGet(int id)
        {
            return TestData.Find(id);
        }

        public int TestDataSave(TestDataDto testDataToSave, string userId)
        {
            var testDataSaved = new TestData
            {
                TestString = testDataToSave.TestString,
                UserId = userId
            };
            TestData.Add(testDataSaved);
            SaveChanges();
            return testDataSaved.Id;
        }

        public string GetAesKeyForUser(string userId)
        {
            var user = Users.Find(userId);
            if (user == null)
            {
                throw new Exception("Couldn't find such user");
            }
            return user.AesSecretKey;
        }
    }
}