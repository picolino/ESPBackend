
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Cryptography;
using System.Text;
using ESPBackend.Dto;
using Newtonsoft.Json;

namespace ESPBackend.Models
{
    public class JsonCryptoFormatter : JsonMediaTypeFormatter
    {
        public override object ReadFromStream(Type type, Stream readStream, Encoding effectiveEncoding, IFormatterLogger formatterLogger)
        {
            var stream = ReadFromStreamInternal(readStream, effectiveEncoding);
            return base.ReadFromStream(type, stream, effectiveEncoding, formatterLogger);
        }

        private Stream ReadFromStreamInternal(Stream readStream, Encoding effectiveEncoding)
        {
            var streamBuffer = new MemoryStream();
            readStream.CopyTo(streamBuffer);

            streamBuffer.Position = 0;
            readStream.Position = 0;

            using (var reader = new StreamReader(readStream))
            {
                var requestString = reader.ReadToEnd();
                var encrypted = JsonConvert.DeserializeObject<RsaEncryptedDataDto>(requestString);

                if (encrypted.IsValid)
                {
                    var rsaKeyContainer = new CspParameters { KeyContainerName = encrypted.KeyContainerGuid };
                    var rsaProvider = new RSACryptoServiceProvider(rsaKeyContainer);

                    var encryptedDataBytes = effectiveEncoding.GetBytes(encrypted.RsaEncryptedData);
                    var decryptedDataBytes = rsaProvider.Decrypt(encryptedDataBytes, false);

                    var decryptedDataString = effectiveEncoding.GetString(decryptedDataBytes);

                    var stream = new MemoryStream();
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(decryptedDataString);
                    }

                    return stream;
                }
            }

            return streamBuffer;
        }
    }
}