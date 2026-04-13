using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModoDemoMVP.Data;
using ModoDemoMVP.DTOs;
using ModoDemoMVP.Models;
using ModoDemoMVP.Services;

namespace ModoDemoMVP.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ModoService _modoService;
        public IndexModel(AppDbContext context, ModoService modoService)
        {
            _context = context;
            _modoService = modoService;
        }

        public List<Pago> Pagos { get; set; } = new();
        public string? QrImageUrl { get; set; }
        public string? PaymentLink { get; set; }
        public async Task OnGetAsync()
        {
            Pagos = await _context.Pagos.ToListAsync();
        }

        public async Task<IActionResult> OnPostCrearAsync() 
        {
            //Crear registro local en estado peniente 
            var pago = new Pago
            {
                Monto = 1000,
                Estado = "Pendiente",
                FechaCreacion = DateTime.UtcNow,
                ExternalReference = Guid.NewGuid().ToString() // referencia unica para correlacionar pagos local y externo
            };

            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();// se genera el id local

            try
            {
                var modoResponse = await _modoService
                     .CrearPagoAsync(pago.ExternalReference!,pago.Monto );

                Console.WriteLine("Deeplik: " + modoResponse.Deeplink);
                Console.WriteLine("Qr: " + modoResponse.Qr);
                //Console.WriteLine("Status:" + modoResponse.Status);
                
                //guardar datos de MODO
                pago.ModoPaymentId = modoResponse.Id;
                pago.QrData = modoResponse.Qr;
                pago.PaymentLink = modoResponse.Deeplink;
                pago.Estado = "Pendiente";
                pago.FechaActualizacion = DateTime.UtcNow;

                QrImageUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=300x300&data={Uri.EscapeDataString(modoResponse.Qr)}";
                PaymentLink = modoResponse.Deeplink;
            }
            catch (Exception ex)
            {
                pago.Estado = "Error: " + ex.Message;
                pago.FechaActualizacion = DateTime.UtcNow;
                
            }

            await _context.SaveChangesAsync();

            Pagos = await _context.Pagos.ToListAsync(); 

            return Page(); // recargar la pagina para mostrar el nuevo pago creado
            
            //return RedirectToPage();
        }
        public async Task<IActionResult> OnGetTestTokenAsync()
        {
            var token = await _modoService.ObtenerTokenAsync();
            return Content(token);
        }
    }
}
