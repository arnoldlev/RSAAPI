using System.Text.Json.Serialization;

namespace RSAAPI.Models
{
    public class UserAttribute
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    // Define the UserResponse class
    public class UserResponse
    {
        [JsonPropertyName("userAttributes")]
        public List<UserAttribute> UserAttributes { get; set; }
    }
}
