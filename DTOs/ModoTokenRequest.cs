using System.Text.Json.Serialization;

namespace ModoDemoMVP.DTOs
{
    public class ModoTokenRequest
    {
        [JsonPropertyName("Username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("Password")]
        public string Password { get; set; } = string.Empty;
    }
}
