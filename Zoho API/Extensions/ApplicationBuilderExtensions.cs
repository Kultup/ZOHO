using Zoho_API.Middleware;

namespace Zoho_API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication ConfigureRequestPipeline(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();


        app.UseHttpsRedirection();
        app.UseMiddleware<ApiKeyAuthMiddleware>();
        app.MapControllers();

        return app;
    }
}
