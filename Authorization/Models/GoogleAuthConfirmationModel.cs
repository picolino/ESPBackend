namespace Authorization.Models
{
    public class GoogleAuthConfirmationModel
    {
        public string InputCode { get; set; }
        public string SecretKey { get; set; }
        public string Barcode { get; set; }
    }
}