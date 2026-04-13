using System.Text.Json;

namespace ModoDemoMVP.Services
{
    public class ModoKeysService
    {
        private readonly HttpClient _httpClient;

        public JsonDocument Keys { get; set; }

        public ModoKeysService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task LoadKeysAsync()
        {
            var url = "https://merchants.preprod.playdigital.com.ar/v2/payment-requests/.well-known/jwks.json";

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ModoDemoMVP");

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            Keys = JsonDocument.Parse(json);
        }
    }
}
