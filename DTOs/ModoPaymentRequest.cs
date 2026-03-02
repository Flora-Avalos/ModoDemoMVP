namespace ModoDemoMVP.DTOs
{
    public class ModoPaymentRequest
    {
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "ARS";
        public string CcCode { get; set; } = string.Empty;
        public string ProcessorCode { get; set; } = string.Empty;
        public string ExternalIntentionId { get; set; } = string.Empty;

        // Otros campos opcionales pueden ser agregados aquí según la documentación de Modo
        public string? WebhookNotification { get; set; } 
        public string? Message { get; set; }
    }
}
