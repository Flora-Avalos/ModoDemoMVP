using System.Text.Json;

namespace ModoDemoMVP.Models
{
    public class MosoWebhookRequest
    {
        public string Id { get; set; }
        public string status { get; set; }
        public string external_intention_id { get; set; }
        public string message { get; set; }
        public JsonElement signature { get; set; }
    }
}
