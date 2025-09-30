using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace citas.Services
{
    public class WhatsAppService
    {
        private readonly HttpClient _httpClient;
        private readonly string _whatsAppApiUrl;
        private readonly string _token;

        public WhatsAppService(HttpClient httpClient, string whatsAppApiUrl, string token)
        {
            _httpClient = httpClient;
            _whatsAppApiUrl = whatsAppApiUrl;
            _token = token;
        }

        public async Task SendMessageAsync(string to, string message)
        {
            var payload = new
            {
                messaging_product = "whatsapp",
                to,
                text = new { body = message }
            };
            var request = new HttpRequestMessage(HttpMethod.Post, _whatsAppApiUrl)
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {_token}");
            await _httpClient.SendAsync(request);
        }
    }
}
