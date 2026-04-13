using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModoDemoMVP.Data;
using ModoDemoMVP.DTOs;

namespace ModoDemoMVP.Controller
{
    [ApiController]
    [Route("api/webhook/modo")]
    public class ModoWebhookController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ModoWebhookController(AppDbContext context)
        {
            _context = context;
        }
         
        [HttpPost]
        public async Task<IActionResult> RecibirWebhook([FromBody] ModoWebhookNotification notification) 
        {
            Console.WriteLine($"Webhook recibido: PaymentId={notification.PaymentId}, ExternalIntentionId={notification.ExternalIntentionId}, Status={notification.Status}");

            var pago = await _context.Pagos
                .FirstOrDefaultAsync(p => 
                    p.ExternalReference == notification.ExternalIntentionId);

            if (pago == null)
                return Ok();

            switch (notification.Status)
            {
                case "SCANNED":
                    Console.WriteLine("QR escaneado");
                    break;

                case "PROCESSING":
                    Console.WriteLine("Pago en proceso");
                    pago.Estado = "En proceso";
                    break;

                case "ACCEPTED":
                    Console.WriteLine("Pago aprobado");
                    pago.Estado = "Aprobado";
                    break;

                case "REJECTED":
                    Console.WriteLine("Pago rechazado");
                    pago.Estado = "Rechazado";
                    break;
            }

            pago.Estado = notification.Status;
            pago.FechaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok("Webhook recibido");
        }
    }
}
