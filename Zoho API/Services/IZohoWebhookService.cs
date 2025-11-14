namespace Zoho_API.Services;

public interface IZohoWebhookService
{
    Task SendEventToZohoAsync(object eventData);
}
