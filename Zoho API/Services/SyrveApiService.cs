using System.Text;
using System.Text.Json;
using Serilog;
using Zoho_API.Models;

namespace Zoho_API.Services;

public class SyrveApiService(HttpClient httpClient, IConfiguration configuration)
    : ISyrveApiService
{
    public async Task<AssignWaiterResponse?> AssignWaiterToReserveAsync(AssignWaiterRequest request)
    {
        try
        {
            string? syrveApiUrl = configuration["SyrveApi:BaseUrl"];
            string? syrveApiKey = configuration["SyrveApi:ApiKey"];
            string? assignWaiterEndpoint = configuration["SyrveApi:AssignWaiterEndpoint"];

            if (string.IsNullOrEmpty(syrveApiUrl) || string.IsNullOrEmpty(assignWaiterEndpoint))
            {
                Log.Error("Syrve API configuration is missing");
                throw new InvalidOperationException("Syrve API configuration is missing");
            }

            string requestUrl = $"{syrveApiUrl.TrimEnd('/')}/{assignWaiterEndpoint.TrimStart('/')}";

            Log.Information("Calling Syrve API: {Url}", requestUrl);
            Log.Information("Request payload: {Payload}", JsonSerializer.Serialize(request));

            HttpRequestMessage httpRequest = new(HttpMethod.Post, requestUrl)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json")
            };

            if (!string.IsNullOrEmpty(syrveApiKey))
            {
                httpRequest.Headers.Add("X-API-Key", syrveApiKey);
            }

            HttpResponseMessage response = await httpClient.SendAsync(httpRequest);
            string responseContent = await response.Content.ReadAsStringAsync();

            Log.Information("Syrve API response status: {StatusCode}", response.StatusCode);
            Log.Information("Syrve API response: {Response}", responseContent);

            if (!response.IsSuccessStatusCode)
            {
                Log.Error("Syrve API returned error: {StatusCode} - {Response}",
                    response.StatusCode, responseContent);
                throw new HttpRequestException(
                    $"Syrve API returned {response.StatusCode}: {responseContent}");
            }

            AssignWaiterResponse? result = JsonSerializer.Deserialize<AssignWaiterResponse>(
                responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error calling Syrve API for reserve {ReserveId}", request.ReserveId);
            throw;
        }
    }
}