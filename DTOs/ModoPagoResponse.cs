namespace ModoDemoMVP.DTOs
{
    public class ModoPagoResponse
    {
        public string PaymentId { get; set; } = string.Empty;
        public string? ExternalIntentionId { get; set; }
        public string? QrData { get; set; } 
        public string? PaymentLink { get; set; } 
        public string Status { get; set; } = "Pendiente";
    }
}
