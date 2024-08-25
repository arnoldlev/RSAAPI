using Newtonsoft.Json;

namespace RSAAPI.Models
{
    public class LicenseResult
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("user")]
        public string? User { get; set; }
        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("valid")]
        public bool IsValid { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }
    }
}

