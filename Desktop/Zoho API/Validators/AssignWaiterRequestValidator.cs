using Serilog;
using Zoho_API.Models;

namespace Zoho_API.Validators;

public class AssignWaiterRequestValidator : IRequestValidator<AssignWaiterRequest>
{
    public ValidationResult Validate(AssignWaiterRequest request)
    {
        List<string> errors = [];
        if (string.IsNullOrWhiteSpace(request.OrganizationId))
        {
            errors.Add("OrganizationId is required and cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(request.ReserveId))
        {
            errors.Add("ReserveId is required and cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(request.EmployeeId))
        {
            errors.Add("EmployeeId is required and cannot be empty");
        }

        if (!string.IsNullOrWhiteSpace(request.OrganizationId) &&
            !Guid.TryParse(request.OrganizationId, out _))
        {
            errors.Add("OrganizationId must be a valid GUID format");
        }

        if (!string.IsNullOrWhiteSpace(request.ReserveId) &&
            !Guid.TryParse(request.ReserveId, out _))
        {
            errors.Add("ReserveId must be a valid GUID format");
        }

        if (!string.IsNullOrWhiteSpace(request.EmployeeId) &&
            !Guid.TryParse(request.EmployeeId, out _))
        {
            errors.Add("EmployeeId must be a valid GUID format");
        }

        if (errors.Any())
        {
            Log.Warning("Validation failed: {Errors}", string.Join(", ", errors));
            return ValidationResult.Failure(errors.ToArray());
        }

        return ValidationResult.Success();
    }
}
