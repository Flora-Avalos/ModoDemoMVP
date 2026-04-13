using System.Text.Json.Serialization;

namespace ModoDemoMVP.DTOs
{
    public class ModoWebhookNotification
    {
        [JsonPropertyName("id")]
        public string? PaymentId { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("external_intention_id")]
        public string? ExternalIntentionId { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
      
    }
}
