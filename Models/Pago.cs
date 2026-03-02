namespace ModoDemoMVP.Models
{
    public class Pago
    {
        public int Id { get; set; }
        public string? ModoPaymentId { get; set; }
        public string? ExternalReference { get; set; }
        public decimal Monto {  get; set; }
        public string? Estado { get; set; } = "Pendiente";
        public string? QrData { get; set; }
        public string? PaymentLink { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaActualizacion { get; set; }
    }
}
