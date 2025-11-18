using Microsoft.AspNetCore.Mvc;
using Serilog;
using Zoho_API.Builders;
using Zoho_API.Models;
using Zoho_API.Services;
using Zoho_API.Validators;

namespace Zoho_API.Controllers;

[ApiController]
[Route("api")]
public class WaiterController(
    ISyrveApiService syrveApiService,
    IRequestValidator<AssignWaiterRequest> validator,
    IResponseBuilder responseBuilder)
    : ControllerBase
{
    [HttpPost("assign-waiter")]
    [ProducesResponseType(typeof(AssignWaiterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AssignWaiter([FromBody] AssignWaiterRequest request)
    {
        try
        {
            Log.Information("[{Time}] [INFO] /api/assign-waiter called",
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            Log.Information("Request: {Request}",
                System.Text.Json.JsonSerializer.Serialize(request));

            ValidationResult validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                Log.Warning("Validation failed: {Errors}",
                    string.Join(", ", validationResult.Errors));

                ErrorResponse errorResponse = responseBuilder
                    .BuildValidationError(validationResult.Errors);

                Log.Information("Response: {Response}",
                    System.Text.Json.JsonSerializer.Serialize(errorResponse));
                Log.Information("Status: Error");

                return BadRequest(errorResponse);
            }

            AssignWaiterResponse? response = await syrveApiService.AssignWaiterToReserveAsync(request);

            Log.Information("Response: {Response}",
                System.Text.Json.JsonSerializer.Serialize(response));
            Log.Information("Status: Success");

            return Ok(response);
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "HTTP error occurred while calling Syrve API");

            ErrorResponse errorResponse = responseBuilder.BuildHttpError(ex.Message);

            Log.Information("Response: {Response}",
                System.Text.Json.JsonSerializer.Serialize(errorResponse));
            Log.Information("Status: Error");

            return StatusCode(StatusCodes.Status502BadGateway, errorResponse);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error in /api/assign-waiter");

            ErrorResponse errorResponse = responseBuilder.BuildInternalError(ex.Message);

            Log.Information("Response: {Response}",
                System.Text.Json.JsonSerializer.Serialize(errorResponse));
            Log.Information("Status: Error");

            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }
}