namespace ESPBackend.Dto
{
    public class RsaEncryptedDataDto
    {
        public string KeyContainerGuid { get; set; }
        public string RsaEncryptedData { get; set; }

        public bool IsValid => KeyContainerGuid != null && RsaEncryptedData != null;
    }
}