using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using System.Text;

namespace RSAAPI.Services
{
    public class EncryptionService
    {
        private readonly string _keyId;
        private readonly IAmazonKeyManagementService _kmsClient;

        public EncryptionService(string keyId, IAmazonKeyManagementService kmsClient)
        {
            _keyId = keyId;
            _kmsClient = kmsClient;
        }

        public async Task<string> EncryptData(string plainText)
        {
            var encryptRequest = new EncryptRequest
            {
                KeyId = _keyId,
                Plaintext = new MemoryStream(Encoding.UTF8.GetBytes(plainText))
            };


            try
            {
                var response = await _kmsClient.EncryptAsync(encryptRequest);
                return Convert.ToBase64String(response.CiphertextBlob.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Attempted " + plainText);
                return "";
            }
        }

        public async Task<string> DecryptData(string encryptedText)
        {
            var decryptRequest = new DecryptRequest
            {
                CiphertextBlob = new MemoryStream(Convert.FromBase64String(encryptedText))
            };

            var response = await _kmsClient.DecryptAsync(decryptRequest);
            return Encoding.UTF8.GetString(response.Plaintext.ToArray());
        }
    }
}
