using Microsoft.AspNetCore.Mvc;
using ModoDemoMVP.Data;
using ModoDemoMVP.DTOs;
using ModoDemoMVP.Models;
using ModoDemoMVP.Services;

namespace ModoDemoMVP.Controllers
{
    [ApiController]
    [Route("api/pagos")]
    public class PagosController : ControllerBase
    {
        private readonly ModoService _modoService;
        private readonly AppDbContext _context;

        public PagosController(ModoService modoService, AppDbContext context)
        {
            _modoService = modoService;
            _context = context;
        }

        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] CrearPagoRequest request)
        {
            var externalId = Guid.NewGuid().ToString();

            var modoResponse = await _modoService.CrearPagoAsync(externalId, request.Monto);

            var pago = new Pago
            {
                ExternalReference = externalId,
                ModoPaymentId = modoResponse.Id,
                Monto = request.Monto,
                Estado = "Pendiente",
                QrData = modoResponse.Qr,
                PaymentLink = modoResponse.Deeplink,
                FechaCreacion = DateTime.UtcNow
            };
            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = modoResponse.Id,
                qr = modoResponse.Qr,
                deeplink = modoResponse.Deeplink,
            });
        }



    }
}
