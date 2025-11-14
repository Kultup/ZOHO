using Zoho_API.Models;

namespace Zoho_API.Builders;

public class ResponseBuilder : IResponseBuilder
{
    public ErrorResponse BuildValidationError(List<string> errors)
    {
        return new ErrorResponse
        {
            Status = "error",
            Message = string.Join("; ", errors),
            ErrorType = "Validation",
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    public ErrorResponse BuildHttpError(string message)
    {
        return new ErrorResponse
        {
            Status = "error",
            Message = $"Failed to communicate with external service: {message}",
            ErrorType = "HTTP",
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    public ErrorResponse BuildInternalError(string message)
    {
        return new ErrorResponse
        {
            Status = "error",
            Message = $"An internal error occurred: {message}",
            ErrorType = "Exception",
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    public ErrorResponse BuildAuthenticationError(string message)
    {
        return new ErrorResponse
        {
            Status = "error",
            Message = message,
            ErrorType = "Authentication",
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    public object BuildSuccess(string message)
    {
        return new
        {
            status = "success",
            message,
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }
}
