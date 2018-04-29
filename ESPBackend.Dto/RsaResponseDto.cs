namespace ESPBackend.Dto
{
    public class RsaResponseDto
    {
        public string ContainerGuid { get; set; }
        public string RsaPublicKey { get; set; }

        public override string ToString()
        {
            return $"{nameof(ContainerGuid)}: {ContainerGuid}, {nameof(RsaPublicKey)}: {RsaPublicKey}";
        }
    }
}