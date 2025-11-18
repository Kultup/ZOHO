using Zoho_API.Builders;
using Zoho_API.Models;
using Zoho_API.Services;
using Zoho_API.Validators;

namespace Zoho_API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpClient<ISyrveApiService, SyrveApiService>();
        services.AddHttpClient<IZohoWebhookService, ZohoWebhookService>();
        services.AddScoped<IRequestValidator<AssignWaiterRequest>, AssignWaiterRequestValidator>();
        services.AddSingleton<IResponseBuilder, ResponseBuilder>();

        return services;
    }

    public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddOpenApi();

        return services;
    }
}