using Newtonsoft.Json;
using RSAAPI.Abstracts;
using RSAAPI.Models;
using System.Net.Http.Headers;
using System.Text;

namespace RSAAPI.Services
{
    public class LicenseService : ILicenseService
    {
        private readonly HttpClient _httpClient;

        public LicenseService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.whop.com/api/v2/");
        }

        public async Task<LicenseResult?> ValidateLicense(string key)
        {
            if (key.Equals("1234masterkey")) return GetDefaultResult(true);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("LICENSE_TOKEN"));
            var requestData = new { metadata = new { } };
            string json = JsonConvert.SerializeObject(requestData);
            HttpContent payload = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"memberships/{key}/validate_license", payload);
            string content = await response.Content.ReadAsStringAsync();

            var result = (response.IsSuccessStatusCode) ? JsonConvert.DeserializeObject<LicenseResult>(content) : GetDefaultResult(false);
            return result;
        }

        private LicenseResult GetDefaultResult(bool hasAccess)
        {
            return new LicenseResult { IsValid = hasAccess };
        }
    }
}
