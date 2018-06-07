using System;
using System.Threading.Tasks;
using System.Web;
using Authorization.Models;
using Authorization.Providers;
using Base32;
using OtpSharp;
using Shared.Logging;

namespace Authorization.Services.Modules
{
    public class GoogleAuthenticatorModule : ModuleBase
    {
        private const string CurrentClassName = nameof(GoogleAuthenticatorModule);
        private const string QrCodeImageGeneratorUrlPrefix = "http://qrcode.kaywa.com/img.php?s=4&d=";
        private readonly string issuerName;

        public GoogleAuthenticatorModule(ILogger logger, AuthRepository repository, string issuerName) : base(logger, repository)
        {
            this.issuerName = issuerName;
        }

        public async Task<GoogleAuthConfirmationModel> GetSecret(string userName)
        {
            var secretKey = KeyGeneration.GenerateRandomKey(20);
            var barcodeUrl = KeyUrl.GetTotpUrl(secretKey, userName) + $"&issuer={issuerName}";

            var model = new GoogleAuthConfirmationModel
                        {
                            Barcode = QrCodeImageGeneratorUrlPrefix + HttpUtility.UrlEncode(barcodeUrl),
                            SecretKey = Base32Encoder.Encode(secretKey)
                        };

            return model;
        }

        public async Task Disable(string userId)
        {
            var user = await Repository.FindById(userId);
            user.IsGoogleAuthenticatorEnabled = false;
            user.GoogleAuthenticatorSecretKey = null;
            await Repository.UpdateUser(user);
        }

        public async Task Confirm(GoogleAuthConfirmationModel confirmationModel, string userId)
        {
            Logger.Debug(CurrentClassName, nameof(Confirm), $"Decoding secret key '{confirmationModel.SecretKey}'");
            var secretKey = Base32Encoder.Decode(confirmationModel.SecretKey);
            
            long timeStepMatched = 0;
            Logger.Debug(CurrentClassName, nameof(Confirm), $"Generating TOTP-key");
            var otp = new Totp(secretKey);

            if (otp.VerifyTotp(confirmationModel.InputCode, out timeStepMatched))
            {
                var user = await Repository.FindById(userId);
                user.IsGoogleAuthenticatorEnabled = true;
                user.GoogleAuthenticatorSecretKey = confirmationModel.SecretKey;
                await Repository.UpdateUser(user);
            }

            throw new Exception("Code is not valid");
        }
    }
}