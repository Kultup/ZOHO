using System.ComponentModel.DataAnnotations;

namespace Zoho_API.Models;

public class AssignWaiterRequest
{
    [Required(ErrorMessage = "OrganizationId is required")]
    public string OrganizationId { get; set; } = string.Empty;

    [Required(ErrorMessage = "ReserveId is required")]
    public string ReserveId { get; set; } = string.Empty;

    [Required(ErrorMessage = "EmployeeId is required")]
    public string EmployeeId { get; set; } = string.Empty;
}
