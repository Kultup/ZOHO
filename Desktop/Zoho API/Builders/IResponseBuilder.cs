using Zoho_API.Models;

namespace Zoho_API.Builders;

public interface IResponseBuilder
{
    ErrorResponse BuildValidationError(List<string> errors);
    ErrorResponse BuildHttpError(string message);
    ErrorResponse BuildInternalError(string message);
    ErrorResponse BuildAuthenticationError(string message);
    object BuildSuccess(string message);
}
