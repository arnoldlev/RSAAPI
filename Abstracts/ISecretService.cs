using Newtonsoft.Json.Linq;

namespace RSAAPI.Abstracts
{
    public interface ISecretService
    {
        Task<string> GetSecretValueAsync(string secretName);
    }
}
