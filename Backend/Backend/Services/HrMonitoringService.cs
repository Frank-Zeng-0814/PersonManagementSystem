using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

/// <summary>
/// Background service that monitors HR-related entities and performs automated tasks.
/// Runs every hour to:
/// - End expired contracts
/// - Complete finished leaves
/// - Notify about expiring contracts
/// - Notify about upcoming leaves
/// </summary>
public class HrMonitoringService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HrMonitoringService> _logger;
    private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(1);

    public HrMonitoringService(
        IServiceProvider serviceProvider,
        ILogger<HrMonitoringService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HR Monitoring Service started");

        // Run immediately on startup, then every hour
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformMonitoringTasksAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during HR monitoring tasks");
            }

            // Wait for next execution
            try
            {
                await Task.Delay(CheckInterval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Service is stopping
                break;
            }
        }

        _logger.LogInformation("HR Monitoring Service stopped");
    }

    private async Task PerformMonitoringTasksAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting HR monitoring tasks at {Time}", DateTime.UtcNow);

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var notificationPublisher = scope.ServiceProvider.GetRequiredService<INotificationPublisher>();

        var today = DateTime.UtcNow.Date;

        // Task 1: End expired contracts
        await EndExpiredContractsAsync(context, notificationPublisher, today, cancellationToken);

        // Task 2: Complete finished leaves
        await CompleteFinishedLeavesAsync(context, notificationPublisher, today, cancellationToken);

        // Task 3: Notify about expiring contracts (30 days)
        await NotifyExpiringContractsAsync(context, notificationPublisher, today, cancellationToken);

        // Task 4: Notify about upcoming leaves (7 days)
        await NotifyUpcomingLeavesAsync(context, notificationPublisher, today, cancellationToken);

        _logger.LogInformation("Completed HR monitoring tasks at {Time}", DateTime.UtcNow);
    }

    private async Task EndExpiredContractsAsync(
        AppDbContext context,
        INotificationPublisher notificationPublisher,
        DateTime today,
        CancellationToken cancellationToken)
    {
        var expiredContracts = await context.EmploymentContracts
            .Include(c => c.Employee)
            .Where(c => c.Status == ContractStatus.Active
                && c.EndDate.HasValue
                && c.EndDate.Value < today)
            .ToListAsync(cancellationToken);

        if (!expiredContracts.Any())
        {
            _logger.LogDebug("No expired contracts found");
            return;
        }

        _logger.LogInformation("Found {Count} expired contracts to process", expiredContracts.Count);

        foreach (var contract in expiredContracts)
        {
            contract.Status = ContractStatus.Ended;

            // Check if employee has any other active contracts
            var hasOtherActiveContracts = await context.EmploymentContracts
                .AnyAsync(c => c.EmployeeId == contract.EmployeeId
                    && c.Id != contract.Id
                    && c.Status == ContractStatus.Active,
                    cancellationToken);

            if (!hasOtherActiveContracts && contract.Employee != null)
            {
                contract.Employee.Status = EmployeeStatus.Inactive;
                _logger.LogInformation(
                    "Employee {EmployeeName} (ID: {EmployeeId}) set to Inactive (no active contracts)",
                    contract.Employee.FullName,
                    contract.EmployeeId);
            }

            // Publish notification
            if (contract.Employee != null)
            {
                await notificationPublisher.PublishContractEndedAsync(new ContractNotificationDto
                {
                    ContractId = contract.Id,
                    EmployeeId = contract.EmployeeId,
                    EmployeeName = contract.Employee.FullName,
                    EmployeeEmail = contract.Employee.Email,
                    EndDate = contract.EndDate!.Value,
                    EmploymentType = contract.EmploymentType,
                    BaseSalary = contract.BaseSalary,
                    DaysUntilExpiry = 0
                }, cancellationToken);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Ended {Count} expired contracts", expiredContracts.Count);
    }

    private async Task CompleteFinishedLeavesAsync(
        AppDbContext context,
        INotificationPublisher notificationPublisher,
        DateTime today,
        CancellationToken cancellationToken)
    {
        var finishedLeaves = await context.LeaveRequests
            .Include(lr => lr.Employee)
            .Where(lr => lr.Status == LeaveRequestStatus.Approved
                && lr.EndDate < today)
            .ToListAsync(cancellationToken);

        if (!finishedLeaves.Any())
        {
            _logger.LogDebug("No finished leaves found");
            return;
        }

        _logger.LogInformation("Found {Count} finished leaves to complete", finishedLeaves.Count);

        foreach (var leave in finishedLeaves)
        {
            leave.Status = LeaveRequestStatus.Completed;

            // Publish notification
            if (leave.Employee != null)
            {
                await notificationPublisher.PublishLeaveCompletedAsync(new LeaveNotificationDto
                {
                    LeaveRequestId = leave.Id,
                    EmployeeId = leave.EmployeeId,
                    EmployeeName = leave.Employee.FullName,
                    EmployeeEmail = leave.Employee.Email,
                    Type = leave.Type,
                    StartDate = leave.StartDate,
                    EndDate = leave.EndDate,
                    Reason = leave.Reason,
                    DaysUntilStart = 0
                }, cancellationToken);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Completed {Count} finished leaves", finishedLeaves.Count);
    }

    private async Task NotifyExpiringContractsAsync(
        AppDbContext context,
        INotificationPublisher notificationPublisher,
        DateTime today,
        CancellationToken cancellationToken)
    {
        var thirtyDaysFromNow = today.AddDays(30);

        var expiringContracts = await context.EmploymentContracts
            .Include(c => c.Employee)
            .Where(c => c.Status == ContractStatus.Active
                && c.EndDate.HasValue
                && c.EndDate.Value >= today
                && c.EndDate.Value <= thirtyDaysFromNow)
            .ToListAsync(cancellationToken);

        if (!expiringContracts.Any())
        {
            _logger.LogDebug("No expiring contracts found (next 30 days)");
            return;
        }

        _logger.LogInformation(
            "Found {Count} contracts expiring in the next 30 days",
            expiringContracts.Count);

        foreach (var contract in expiringContracts)
        {
            if (contract.Employee == null) continue;

            var daysUntilExpiry = (contract.EndDate!.Value - today).Days;

            await notificationPublisher.PublishContractExpiringAsync(new ContractNotificationDto
            {
                ContractId = contract.Id,
                EmployeeId = contract.EmployeeId,
                EmployeeName = contract.Employee.FullName,
                EmployeeEmail = contract.Employee.Email,
                EndDate = contract.EndDate.Value,
                DaysUntilExpiry = daysUntilExpiry,
                EmploymentType = contract.EmploymentType,
                BaseSalary = contract.BaseSalary
            }, cancellationToken);
        }
    }

    private async Task NotifyUpcomingLeavesAsync(
        AppDbContext context,
        INotificationPublisher notificationPublisher,
        DateTime today,
        CancellationToken cancellationToken)
    {
        var sevenDaysFromNow = today.AddDays(7);

        var upcomingLeaves = await context.LeaveRequests
            .Include(lr => lr.Employee)
            .Where(lr => lr.Status == LeaveRequestStatus.Approved
                && lr.StartDate >= today
                && lr.StartDate <= sevenDaysFromNow)
            .ToListAsync(cancellationToken);

        if (!upcomingLeaves.Any())
        {
            _logger.LogDebug("No upcoming leaves found (next 7 days)");
            return;
        }

        _logger.LogInformation(
            "Found {Count} leaves starting in the next 7 days",
            upcomingLeaves.Count);

        foreach (var leave in upcomingLeaves)
        {
            if (leave.Employee == null) continue;

            var daysUntilStart = (leave.StartDate - today).Days;

            await notificationPublisher.PublishUpcomingLeaveAsync(new LeaveNotificationDto
            {
                LeaveRequestId = leave.Id,
                EmployeeId = leave.EmployeeId,
                EmployeeName = leave.Employee.FullName,
                EmployeeEmail = leave.Employee.Email,
                Type = leave.Type,
                StartDate = leave.StartDate,
                EndDate = leave.EndDate,
                DaysUntilStart = daysUntilStart,
                Reason = leave.Reason
            }, cancellationToken);
        }
    }
}
