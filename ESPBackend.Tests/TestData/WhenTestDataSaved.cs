#region Usings

using ESPBackend.Dto;
using NUnit.Framework;

#endregion

namespace ESPBackend.Tests.TestData
{
    public class WhenTestDataSaved : TestDataServiceTestBase
    {
        [Test]
        public void DataReallySaved()
        {
            var userId = "0000-0000-0000-0000";
            var testString = "test string content";
            var testData = new TestDataDto
                           {
                               TestString = testString
                           };

            var savedId = TestDataService.Save(testData, userId);

            var expectedData = new DataAccessLayer.TestData { Id = savedId, TestString = testString, UserId = userId };
            Assert.AreEqual(TestDataRepository.GetTestDataById(savedId), expectedData);
        }

        [Test]
        public void DataCanBeGet()
        {
            var userId = "0000-0000-0000-0000";
            var testString = "test string content";
            var testData = new TestDataDto
                           {
                               TestString = testString
                           };
            var savedId = TestDataRepository.SaveTestData(testData, userId);
            var gettedData = TestDataService.GetBy(savedId);
            
            Assert.AreEqual(gettedData, TestDataRepository.GetTestDataById(savedId));
        }
    }
}