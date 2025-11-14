using Serilog;

namespace Zoho_API.Extensions;

public static class SerilogExtensions
{
    /// <summary>
    /// Configures Serilog logging with file and console output
    /// </summary>
    public static IHostBuilder ConfigureSerilog(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .WriteTo.Console()
                .WriteTo.File(
                    path: "logs/syrve-sync.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}");
        });
    }
}
