using Microsoft.AspNetCore.Mvc;
using ModoDemoMVP.Data;
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
        public async Task<IActionResult> Crear(decimal monto)
        {
            var externalId = Guid.NewGuid().ToString();

            var pago = new Pago
            {
                ExternalReference = externalId,
                Monto = monto,
                Estado = "Pendiente",
            };
            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            return Ok(new {externalId});
        }



    }
}
