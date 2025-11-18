using System.Text.Json;

namespace Zoho_API.Models;

public class SyrveWebhookEvent
{
    public string EventType { get; set; } = string.Empty;
    public string OrganizationId { get; set; } = string.Empty;
    public JsonElement EventInfo { get; set; }
}
