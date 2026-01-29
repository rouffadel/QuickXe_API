using DAL.Interface;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DAL.Services
{
    public class MetaWhatsAppService : ITwilioService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public MetaWhatsAppService(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
        }

        public async Task SendOtpWhatsAppAsync(string userPhoneNumber, string otpCode)
        {
            var templateName = _config["MetaWhatsApp:TemplateName"] ?? "otp_verification";
            await SendTemplateMessageAsync(userPhoneNumber, templateName, new[] { otpCode });
        }

        public async Task SendStatusUpdateAsync(string userPhoneNumber, string templateName, string[] parameters)
        {
            await SendTemplateMessageAsync(userPhoneNumber, templateName, parameters);
        }

        private async Task SendTemplateMessageAsync(string userPhoneNumber, string templateName, string[] parameters)
        {
            var accessToken = _config["MetaWhatsApp:AccessToken"];
            var phoneNumberId = _config["MetaWhatsApp:PhoneNumberId"];
            var version = _config["MetaWhatsApp:Version"] ?? "v21.0";
            var languageCode = _config["MetaWhatsApp:LanguageCode"] ?? "en_US";

            var formattedNumber = new string(userPhoneNumber.Where(char.IsDigit).ToArray());
            var url = $"https://graph.facebook.com/{version}/{phoneNumberId}/messages";

            var payload = new
            {
                messaging_product = "whatsapp",
                to = formattedNumber,
                type = "template",
                template = new
                {
                    name = templateName,
                    language = new { code = languageCode },
                    components = new object[]
                    {
                        new
                        {
                            type = "body",
                            parameters = parameters.Select(p => new { type = "text", text = p }).ToArray()
                        },
                        new
                        {
                            type = "button",
                            sub_type = "url",
                            index = "0",
                            parameters = new[]
                            {
                                new
                                {
                                    type = "text",
                                    text = parameters.FirstOrDefault() ?? ""
                                }
                            }
                        }
                    }
                }
            };

            var jsonContent = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = content;

            System.Console.WriteLine($"[Meta WhatsApp] Sending Template '{templateName}' to {formattedNumber}...");

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                System.Console.WriteLine($"[Meta WhatsApp] ERROR: {response.StatusCode} - {responseBody}");
                throw new System.Exception($"Meta API Error: {responseBody}");
            }

            System.Console.WriteLine($"[Meta WhatsApp] SUCCESS: Message sent.");
        }
    }
}


