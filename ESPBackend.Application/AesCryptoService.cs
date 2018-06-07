using System.IO;
using System.Security.Cryptography;
using System.Text;
using ESPBackend.DataAccessLayer.Interfaces;

namespace ESPBackend.Application
{
    public class AesCryptoService
    {
        private readonly ICryptoRepository cryptoRepository;

        public AesCryptoService(ICryptoRepository cryptoRepository)
        {
            this.cryptoRepository = cryptoRepository;
        }

        public string Decrypt(string userId, string source)
        {
            var secret = cryptoRepository.GetAesKeyByUserId(userId);
            if (!string.IsNullOrEmpty(secret))
            {
                var decrypted = DecryptCore(source, secret);
                return decrypted;
            }
            return source;
        }

        private string DecryptCore(string source, string key)
        {
            string plaintext;

            var encodedSourceBytes = Encoding.Default.GetBytes(source);
            var encodedSectetBytes = Encoding.Default.GetBytes(key);
            using (var aes = new AesManaged())
            {
                aes.Key = encodedSectetBytes;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.None;
                aes.Mode = CipherMode.ECB;
                
                var decryptor = aes.CreateDecryptor(aes.Key, null);
                
                using (var msDecrypt = new MemoryStream(encodedSourceBytes))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}