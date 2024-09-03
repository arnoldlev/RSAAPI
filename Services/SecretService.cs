using Amazon.SecretsManager.Model;
using Amazon.SecretsManager;
using Newtonsoft.Json.Linq;
using RSAAPI.Abstracts;

namespace RSAAPI.Services
{
    public class SecretService : ISecretService
    {
        private readonly IAmazonSecretsManager _secretsManagerClient;

        public SecretService(IAmazonSecretsManager secretsManagerClient)
        {
            _secretsManagerClient = secretsManagerClient;
        }

        public async Task<string> GetSecretValueAsync(string secretName)
        {
            try
            {
                var request = new GetSecretValueRequest
                {
                    SecretId = secretName
                };

                var response = await _secretsManagerClient.GetSecretValueAsync(request);

                if (response.SecretString != null)
                {
                    return response.SecretString;
                }
                else
                {
                    throw new Exception("Secret binary data is not supported in this example.");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error retrieving secret: {e.Message}");
            }
        }
    }

}
