using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModoDemoMVP.Data;
using ModoDemoMVP.DTOs;

namespace ModoDemoMVP.Controller
{
    [ApiController]
    [Route("api/webhooks/modo")]
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

            var pago = await _context.Pagos
                .FirstOrDefaultAsync(p => 
                    p.ExternalReference == notification.ExternalIntentionId);

            if (pago == null)
                return Ok();

            pago.Estado = notification.Status;
            pago.FechaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok("Webhook recibido");
        }
    }
}
