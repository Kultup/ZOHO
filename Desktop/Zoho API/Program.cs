using Serilog;
using Zoho_API.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureSerilog();
builder.Services.AddApplicationServices();
builder.Services.AddApiConfiguration();
WebApplication app = builder.Build();
app.ConfigureRequestPipeline();
app.Run();

Log.CloseAndFlush();