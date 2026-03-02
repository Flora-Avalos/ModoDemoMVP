using Microsoft.AspNetCore.Mvc;
using ModoDemoMVP.DTOs;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace ModoDemoMVP.Services
{
    public class ModoService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        private string? _accessToken; // cache del token de acceso
        private DateTime _tokenExpiration; // para controlar la expiración del token

        public ModoService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        // método para obtener el token de acceso, con caching simple para evitar llamadas innecesarias a la API de autenticación de Modo
        public async Task<string> ObtenerTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken) &&
                _tokenExpiration > DateTime.UtcNow)
            {
                return _accessToken; // retornar token cacheado si no ha expirado
            }

            var request = new ModoTokenRequest
            {
                Username = _config["PLAYDIGITAL SA-318979-preprod"]!,
                Password = _config["318979-P75V/QLKfVKX"]!
            };

            var response = await _httpClient.PostAsJsonAsync(
                "/v2/stores/companies/token", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error obteniendo token: {error}");
            }
            // parseamos la respuesta y cacheamos el token y su expiración
            var tokenResponse = await response.Content.ReadFromJsonAsync<ModoTokenResponse>();
            
            _accessToken = tokenResponse!.AccessToken;

            _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60); // renovar 1 minuto antes de expirar

            return _accessToken;
        }

        // Crear Payment Request (simulacion con credenciales genericas)
        public async Task<ModoPagoResponse> CrearPagoAsync(decimal monto, string externalReference)
        {
            ////obtener token
            //var token = await ObtenerTokenAsync();

            ////preparar request
            //var paymentRequest = new ModoPaymentRequest
            //{
            //    Description = "Compra de prueba",
            //    Amount = monto,
            //    Currency = "ARS",
            //    CcCode = "1CSI",
            //    ProcessorCode = "P1019", 
            //    ExternalIntentionId = externalReference
            //};

            ////simulacion de llamada a modo (reemplazar con llamada real a la API de modo)
            //await Task.Delay(300);

            //return new ModoPagoResponse
            //{
            //    PaymentId = Guid.NewGuid().ToString(),
            //    QrData = "Simulated QR Data" + Guid.NewGuid(),
            //    PaymentLink = "https://modo.com/simulated-payment-link" + Guid.NewGuid(),
            //    Status = "Pendiente"
            //};

            //se va a generar un QR de prueba con datos simulados, ya que no tenemos acceso a la API real de Modo en este entorno
            await Task.Delay(200); 

            var paymentId = Guid.NewGuid().ToString();
            var externalIntentionId = Guid.NewGuid().ToString();

            //datos q contendra el qr
            var qrContent = $"PAGO|ID:{paymentId}|AMOUNT:{monto}";


            var qrUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=300x300&data={Uri.EscapeDataString(qrContent)}";

            return new ModoPagoResponse
            {
                PaymentId = paymentId,
                ExternalIntentionId = externalIntentionId,
                QrData = qrUrl,
                PaymentLink = $"https://modo.com/simulated-payment-link",
                Status = "Pendiente"


            };

        }
    }
}
