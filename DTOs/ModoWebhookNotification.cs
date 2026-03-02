namespace ModoDemoMVP.DTOs
{
    public class ModoWebhookNotification
    {
        public string? PaymentId { get; set; }
        public string? ExternalIntentionId { get; set; }
        public string? Status { get; set; }
    }
}
