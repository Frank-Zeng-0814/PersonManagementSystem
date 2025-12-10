using Backend.DTOs;
using Backend.Models;

namespace Backend.Services;

public interface IEmploymentContractService
{
    Task<EmploymentContract> CreateContractAsync(CreateContractDto dto, CancellationToken cancellationToken = default);
    Task<EmploymentContract?> GetContractByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<EmploymentContract>> GetActiveContractsByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
    Task EndExpiredContractsAsync(CancellationToken cancellationToken = default);
}
