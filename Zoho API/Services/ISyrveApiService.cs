using Zoho_API.Models;

namespace Zoho_API.Services;

public interface ISyrveApiService
{
    Task<AssignWaiterResponse?> AssignWaiterToReserveAsync(AssignWaiterRequest request);
}
