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

            var body = new
            {
                Username = _config["Modo:Username"]!,
                Password = _config["Modo:Password"]!
            };

            var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                "/v2/stores/companies/token");

            httpRequest.Content = JsonContent.Create(body);

            httpRequest.Headers.Add("User-Agent", _config["Modo:UserAgent"]);

            var response = await _httpClient.SendAsync(httpRequest);

            if (response.StatusCode != System.Net.HttpStatusCode.Created)
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

        public async Task<ModoPagoResponse> CrearPagoAsync(string externalId, decimal monto)
        {
            var token = await ObtenerTokenAsync();

            var body = new
            {
                description = "Pago desde MVP",
                amount = monto,
                currency = "ARS",
                cc_code = "1CSI",
                processor_code = _config["Modo:ProcessorCode"],
                external_intention_id = externalId,
                webhook_notification_id  = _config["Modo:WebhookUrl"]
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "/v2/payment-requests/");

            request.Content = JsonContent.Create(body);

            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Headers.Add("User-Agent", _config["Modo:UserAgent"]);

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error creando pago: {error}");
            }

            var pagoResponse = await response.Content.ReadFromJsonAsync<ModoPagoResponse>();

            return pagoResponse!;

        }
    }
}
