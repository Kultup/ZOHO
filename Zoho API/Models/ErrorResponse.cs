namespace Zoho_API.Models;

public class ErrorResponse
{
    public string Status { get; set; } = "error";
    public string Message { get; set; } = string.Empty;
    public string? ErrorType { get; set; }
    public string Timestamp { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
}
