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
                FechaCreacion = DateTime.Now,
                ExternalReference = Guid.NewGuid().ToString() // referencia unica para correlacionar pagos local y externo
            };

            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();// se genera el id local

            try
            {
                var modoResponse = await _modoService
                     .CrearPagoAsync(pago.Monto, pago.ExternalReference!);

                Console.WriteLine("PaymentLink: " + modoResponse.PaymentLink);
                Console.WriteLine("QrData: " + modoResponse.QrData);
                Console.WriteLine("Status:" + modoResponse.Status);
                
                //simulacion por ahora
                pago.ModoPaymentId = modoResponse.PaymentId;
                pago.QrData = modoResponse.QrData;
                pago.PaymentLink = modoResponse.PaymentLink;
                pago.Estado = modoResponse.Status;
                pago.FechaActualizacion = DateTime.Now;

                QrImageUrl = modoResponse.QrData;
                PaymentLink = modoResponse.PaymentLink;
            }
            catch (Exception ex)
            {
                pago.Estado = "Error: " + ex.Message;
                pago.FechaActualizacion = DateTime.Now;
                
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
