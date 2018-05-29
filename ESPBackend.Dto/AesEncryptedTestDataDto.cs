namespace ESPBackend.Dto
{
    public class AesEncryptedTestDataDto : TestDataDto
    {
        public string AesEncryptedData { get; set; }

        public bool IsEncryptedData => !string.IsNullOrEmpty(AesEncryptedData);
    }
}