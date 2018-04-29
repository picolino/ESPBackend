namespace ESPBackend.Dto
{
    public class RsaResponseDto
    {
        public string KeyContainerGuid { get; set; }
        public string RsaPublicKey { get; set; }

        public override string ToString()
        {
            return $"{nameof(KeyContainerGuid)}: {KeyContainerGuid}, {nameof(RsaPublicKey)}: {RsaPublicKey}";
        }
    }
}