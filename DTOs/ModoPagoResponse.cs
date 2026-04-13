using System.Text.Json.Serialization;

namespace ModoDemoMVP.DTOs
{
    public class ModoPagoResponse
    {
        //public string PaymentId { get; set; } = string.Empty;
        //public string? ExternalIntentionId { get; set; }
        //public string? QrData { get; set; } 
        //public string? PaymentLink { get; set; } 
        //public string Status { get; set; } = "Pendiente";

        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("qr")]
        public string Qr { get; set; }

        [JsonPropertyName("deeplink")]
        public string Deeplink { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime Created { get; set; }

        [JsonPropertyName("expiration_at")]
        public long ExpirationAt { get; set; }

        [JsonPropertyName("sub_payments")]
        public object? SubPayments { get; set; }

        [JsonPropertyName("best_installemnt")]
        public BestInstallment? BestInstallment { get; set; }

    }

    public class BestInstallment
        {
            [JsonPropertyName("quantity")]
            public int? Quantity { get; set; }
    
            [JsonPropertyName("has_interest")]
            public bool? HasInterest { get; set; }
    }
}
