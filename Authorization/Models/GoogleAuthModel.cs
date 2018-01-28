namespace Authorization.Models
{
    public class GoogleAuthModel
    {
        public string InputCode { get; set; }
        public string SecretKey { get; set; }
        public string Barcode { get; set; }
    }
}