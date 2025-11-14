using System.Net;
using System.Text.Json;
using Serilog;
using Zoho_API.Builders;
using Zoho_API.Models;

namespace Zoho_API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IResponseBuilder responseBuilder)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex, responseBuilder);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        IResponseBuilder responseBuilder)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        ErrorResponse errorResponse = responseBuilder.BuildInternalError(exception.Message);

        string json = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(json);
    }
}
