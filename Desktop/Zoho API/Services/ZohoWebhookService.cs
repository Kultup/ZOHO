using System.Text;
using System.Text.Json;
using Serilog;

namespace Zoho_API.Services;

public class ZohoWebhookService(HttpClient httpClient, IConfiguration configuration)
    : IZohoWebhookService
{
    public async Task SendEventToZohoAsync(object eventData)
    {
        try
        {
            string? zohoWebhookUrl = configuration["ZohoWebhook:Url"];

            if (string.IsNullOrEmpty(zohoWebhookUrl))
            {
                Log.Error("Zoho webhook URL is not configured");
                throw new InvalidOperationException("Zoho webhook URL is not configured");
            }

            string payload = JsonSerializer.Serialize(eventData);

            Log.Information("Sending event to Zoho webhook: {Url}", zohoWebhookUrl);
            Log.Information("Payload: {Payload}", payload);

            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, zohoWebhookUrl)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = await httpClient.SendAsync(httpRequest);
            string responseContent = await response.Content.ReadAsStringAsync();

            Log.Information("Zoho webhook response status: {StatusCode}", response.StatusCode);
            Log.Information("Zoho webhook response: {Response}", responseContent);

            if (!response.IsSuccessStatusCode)
            {
                Log.Warning(
                    "Zoho webhook returned non-success status: {StatusCode} - {Response}",
                    response.StatusCode, responseContent);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error sending event to Zoho webhook");
        }
    }
}