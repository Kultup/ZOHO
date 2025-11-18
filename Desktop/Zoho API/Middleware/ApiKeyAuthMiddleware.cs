using System.Text.Json;
using Serilog;
using Zoho_API.Models;

namespace Zoho_API.Middleware;

public class ApiKeyAuthMiddleware(
    RequestDelegate next,
    IConfiguration configuration)
{
    private const string ApiKeyHeaderName = "X-API-Key";

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/openapi"))
        {
            await next(context);
            return;
        }

        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await next(context);
            return;
        }

        string? providedApiKey = context.Request.Headers[ApiKeyHeaderName].FirstOrDefault();
        string? validApiKey = configuration["ApiKey"];

        if (string.IsNullOrEmpty(validApiKey))
        {
            Log.Warning("API Key is not configured in appsettings.json. Authentication is disabled!");
            await next(context);
            return;
        }

        if (string.IsNullOrEmpty(providedApiKey))
        {
            Log.Warning("Request to {Path} without API Key from {IP}",
                context.Request.Path, context.Connection.RemoteIpAddress);

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            ErrorResponse errorResponse = new ErrorResponse
            {
                Status = "error",
                Message = "API Key is required. Please provide X-API-Key header.",
                ErrorType = "Authentication"
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            return;
        }

        if (providedApiKey != validApiKey)
        {
            Log.Warning("Invalid API Key provided for {Path} from {IP}",
                context.Request.Path, context.Connection.RemoteIpAddress);

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            ErrorResponse errorResponse = new ErrorResponse
            {
                Status = "error",
                Message = "Invalid API Key",
                ErrorType = "Authentication"
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            return;
        }

        Log.Information("Request to {Path} authenticated successfully", context.Request.Path);
        await next(context);
    }
}
