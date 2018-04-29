using System;
using System.Security.Cryptography;
using ESPBackend.Dto;

namespace ESPBackend.Application
{
    public class RsaCryptoService
    {
        public RsaResponseDto GenerateRsaKeyPair()
        {
            var keyContainer = new CspParameters { KeyContainerName = Guid.NewGuid().ToString()};
            var cryptoProvider = new RSACryptoServiceProvider(keyContainer);

            var publicKeyXml = cryptoProvider.ToXmlString(false);

            return new RsaResponseDto
                   {
                       ContainerGuid = keyContainer.KeyContainerName,
                       RsaPublicKey = publicKeyXml
                   };
        }
    }
}