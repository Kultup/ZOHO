using Microsoft.AspNetCore.Mvc;
using Serilog;
using Zoho_API.Builders;
using Zoho_API.Models;
using Zoho_API.Services;

namespace Zoho_API.Controllers;

[ApiController]
[Route("api/syrve")]
public class WebhookController(
    IZohoWebhookService zohoWebhookService,
    IResponseBuilder responseBuilder)
    : ControllerBase
{
    [HttpPost("webhook")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> HandleSyrveWebhook([FromBody] SyrveWebhookEvent webhookEvent)
    {
        try
        {
            Log.Information("[{Time}] [INFO] /api/syrve/webhook called",
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            Log.Information("Request: {Request}",
                System.Text.Json.JsonSerializer.Serialize(webhookEvent));

            await zohoWebhookService.SendEventToZohoAsync(webhookEvent);

            object successResponse = responseBuilder.BuildSuccess(
                "Event received and forwarded to Zoho");

            Log.Information("Response: {Response}",
                System.Text.Json.JsonSerializer.Serialize(successResponse));
            Log.Information("Status: Success");

            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error processing Syrve webhook");

            ErrorResponse errorResponse = responseBuilder.BuildInternalError(
                $"Failed to process webhook: {ex.Message}");

            Log.Information("Response: {Response}",
                System.Text.Json.JsonSerializer.Serialize(errorResponse));
            Log.Information("Status: Error");

            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }
}
