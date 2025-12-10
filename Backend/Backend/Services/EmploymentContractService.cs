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
            throw new DomainException(
                "Employee already has an active contract that overlaps with the specified date range",
                "OVERLAPPING_CONTRACT");
        }

        // Determine contract status
        var today = DateTime.UtcNow.Date;
        var contractStatus = ContractStatus.Active;

        if (dto.EndDate.HasValue && dto.EndDate.Value < today)
        {
            contractStatus = ContractStatus.Ended;
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
        return await _context.EmploymentContracts
            .Include(c => c.Employee)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<List<EmploymentContract>> GetActiveContractsByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.EmploymentContracts
            .Where(c => c.EmployeeId == employeeId && c.Status == ContractStatus.Active)
            .OrderByDescending(c => c.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task EndExpiredContractsAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;

        // Find all active contracts that have expired
        var expiredContracts = await _context.EmploymentContracts
            .Where(c => c.Status == ContractStatus.Active && c.EndDate.HasValue && c.EndDate.Value < today)
            .Include(c => c.Employee)
            .ToListAsync(cancellationToken);

        foreach (var contract in expiredContracts)
        {
            contract.Status = ContractStatus.Ended;

            // Check if employee has any other active contracts
            var hasOtherActiveContracts = await _context.EmploymentContracts
                .AnyAsync(c => c.EmployeeId == contract.EmployeeId
                    && c.Id != contract.Id
                    && c.Status == ContractStatus.Active,
                    cancellationToken);

            // If no active contracts remain, set employee to inactive
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

        if (expiredContracts.Any())
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
