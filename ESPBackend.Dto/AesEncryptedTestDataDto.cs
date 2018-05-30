namespace ESPBackend.Dto
{
    public class AesEncryptedTestDataDto : TestDataDto
    {
        public string AesEncryptedData { get; private set; }

        public bool IsEncryptedData => !string.IsNullOrEmpty(AesEncryptedData);

        public override string ToString()
        {
            return $"{nameof(AesEncryptedData)}: {AesEncryptedData}, {nameof(TestString)}: {TestString}";
        }
    }
}