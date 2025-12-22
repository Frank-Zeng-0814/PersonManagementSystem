using Backend.DTOs;
using Backend.Exceptions;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class EmploymentContractService : IEmploymentContractService
{
    private readonly AppDbContext _context;
    private readonly INotificationPublisher _notificationPublisher;

    public EmploymentContractService(
        AppDbContext context,
        INotificationPublisher notificationPublisher)
    {
        _context = context;
        _notificationPublisher = notificationPublisher;
    }

    public async Task<EmploymentContract> CreateContractAsync(CreateContractDto dto, CancellationToken cancellationToken = default)
    {
        // Validate employee exists
        var employee = await _context.Employees.FindAsync(new object[] { dto.EmployeeId }, cancellationToken);
        if (employee == null)
        {
            throw new DomainException($"Employee with ID {dto.EmployeeId} not found", "EMPLOYEE_NOT_FOUND");
        }

        // Validate dates
        if (dto.EndDate.HasValue && dto.EndDate.Value < dto.StartDate)
        {
            throw new DomainException("EndDate must be after StartDate", "INVALID_DATE_RANGE");
        }

        // Check for overlapping active contracts
        var hasOverlap = await _context.EmploymentContracts
            .Where(c => c.EmployeeId == dto.EmployeeId && c.Status == ContractStatus.Active)
            .AnyAsync(c =>
                (dto.StartDate >= c.StartDate && dto.StartDate < (c.EndDate ?? DateTime.MaxValue)) ||
                (dto.EndDate.HasValue && dto.EndDate.Value > c.StartDate && dto.EndDate.Value <= (c.EndDate ?? DateTime.MaxValue)) ||
                (dto.StartDate <= c.StartDate && (!dto.EndDate.HasValue || dto.EndDate.Value >= (c.EndDate ?? DateTime.MaxValue))),
                cancellationToken);

        if (hasOverlap)
        {
            var overlappingContract = await _context.EmploymentContracts
                .Where(c => c.EmployeeId == dto.EmployeeId && c.Status == ContractStatus.Active)
                .Where(c =>
                    (dto.StartDate >= c.StartDate && dto.StartDate < (c.EndDate ?? DateTime.MaxValue)) ||
                    (dto.EndDate.HasValue && dto.EndDate.Value > c.StartDate && dto.EndDate.Value <= (c.EndDate ?? DateTime.MaxValue)) ||
                    (dto.StartDate <= c.StartDate && (!dto.EndDate.HasValue || dto.EndDate.Value >= (c.EndDate ?? DateTime.MaxValue))))
                .FirstOrDefaultAsync(cancellationToken);

            var endDateStr = overlappingContract?.EndDate?.ToString("yyyy-MM-dd") ?? "No end date";
            throw new DomainException(
                $"Contract overlaps with existing contract (Start: {overlappingContract?.StartDate:yyyy-MM-dd}, End: {endDateStr})",
                "OVERLAPPING_CONTRACT");
        }

        // Determine contract status
        var today = DateTime.UtcNow.Date;
        ContractStatus contractStatus;

        if (dto.StartDate > today)
        {
            contractStatus = ContractStatus.Pending;
        }
        else if (dto.EndDate.HasValue && dto.EndDate.Value < today)
        {
            contractStatus = ContractStatus.Ended;
        }
        else
        {
            contractStatus = ContractStatus.Active;
        }

        // Create contract
        var contract = new EmploymentContract
        {
            EmployeeId = dto.EmployeeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            EmploymentType = dto.EmploymentType,
            BaseSalary = dto.BaseSalary,
            Status = contractStatus
        };

        _context.EmploymentContracts.Add(contract);

        // Update employee status if contract starts today or earlier
        if (dto.StartDate <= today && contractStatus == ContractStatus.Active)
        {
            employee.Status = EmployeeStatus.Active;
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Publish notification via SignalR when contract becomes Active
        if (contractStatus == ContractStatus.Active && _notificationPublisher is SignalRNotificationPublisher signalRPublisher)
        {
            await signalRPublisher.PublishContractUpdatedAsync(
                contract.Id,
                employee.Id,
                employee.FullName,
                "Active",
                contract.EndDate,
                $"New employment contract created and is now active",
                cancellationToken);
        }

        return contract;
    }

    public async Task<EmploymentContract?> GetContractByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var contract = await _context.EmploymentContracts
            .Include(c => c.Employee)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (contract != null)
        {
            contract.Status = ComputeContractStatus(contract.StartDate, contract.EndDate);
        }

        return contract;
    }

    public async Task<List<EmploymentContract>> GetActiveContractsByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.EmploymentContracts
            .Where(c => c.EmployeeId == employeeId && c.Status == ContractStatus.Active)
            .OrderByDescending(c => c.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<EmploymentContract>> GetAllContractsByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        var contracts = await _context.EmploymentContracts
            .Where(c => c.EmployeeId == employeeId)
            .OrderByDescending(c => c.StartDate)
            .ToListAsync(cancellationToken);

        foreach (var contract in contracts)
        {
            contract.Status = ComputeContractStatus(contract.StartDate, contract.EndDate);
        }

        return contracts;
    }

    private static ContractStatus ComputeContractStatus(DateTime startDate, DateTime? endDate)
    {
        var today = DateTime.UtcNow.Date;

        if (startDate.Date > today)
            return ContractStatus.Pending;

        if (endDate.HasValue && endDate.Value.Date < today)
            return ContractStatus.Ended;

        return ContractStatus.Active;
    }

    public async Task EndExpiredContractsAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;

        // Activate pending contracts that should start today
        var pendingContracts = await _context.EmploymentContracts
            .Where(c => c.Status == ContractStatus.Pending && c.StartDate <= today)
            .Include(c => c.Employee)
            .ToListAsync(cancellationToken);

        foreach (var contract in pendingContracts)
        {
            contract.Status = ContractStatus.Active;
            contract.Employee.Status = EmployeeStatus.Active;

            if (_notificationPublisher is SignalRNotificationPublisher signalRPublisher)
            {
                await signalRPublisher.PublishContractUpdatedAsync(
                    contract.Id,
                    contract.EmployeeId,
                    contract.Employee.FullName,
                    "Active",
                    contract.EndDate,
                    $"Employment contract is now active",
                    cancellationToken);
            }
        }

        // Find all active contracts that have expired
        var expiredContracts = await _context.EmploymentContracts
            .Where(c => c.Status == ContractStatus.Active && c.EndDate.HasValue && c.EndDate.Value < today)
            .Include(c => c.Employee)
            .ToListAsync(cancellationToken);

        foreach (var contract in expiredContracts)
        {
            contract.Status = ContractStatus.Ended;

            var otherContracts = await _context.EmploymentContracts
                .Where(c => c.EmployeeId == contract.EmployeeId && c.Id != contract.Id)
                .ToListAsync(cancellationToken);

            var hasOtherActiveContracts = otherContracts.Any(c =>
                ComputeContractStatus(c.StartDate, c.EndDate) == ContractStatus.Active);

            if (!hasOtherActiveContracts)
            {
                contract.Employee.Status = EmployeeStatus.Inactive;
            }

            // Publish notification via SignalR when contract ends
            if (_notificationPublisher is SignalRNotificationPublisher signalRPublisher)
            {
                await signalRPublisher.PublishContractUpdatedAsync(
                    contract.Id,
                    contract.EmployeeId,
                    contract.Employee.FullName,
                    "Ended",
                    contract.EndDate,
                    $"Employment contract has ended",
                    cancellationToken);
            }
        }

        if (pendingContracts.Any() || expiredContracts.Any())
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
