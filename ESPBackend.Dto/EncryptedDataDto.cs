namespace ESPBackend.Dto
{
    public class EncryptedDataDto
    {
        public string KeyContainerGuid { get; set; }
        public string EncryptedData { get; set; }

        public bool IsValid => KeyContainerGuid != null && EncryptedData != null;
    }
}