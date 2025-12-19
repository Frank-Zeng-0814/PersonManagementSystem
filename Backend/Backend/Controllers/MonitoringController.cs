using Backend.DTOs;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonitoringController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MonitoringController> _logger;
    private readonly IMemoryCache _cache;
    private const string CACHE_KEY = "LastHrCheckTime";
    private static readonly TimeSpan MinCheckInterval = TimeSpan.FromMinutes(1);

    public MonitoringController(
        IServiceProvider serviceProvider,
        ILogger<MonitoringController> logger,
        IMemoryCache cache)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _cache = cache;
    }

    [HttpPost("trigger-hr-checks")]
    public async Task<IActionResult> TriggerHrChecks(CancellationToken cancellationToken)
    {
        try
        {
            // Check if we've run recently (rate limiting)
            if (_cache.TryGetValue(CACHE_KEY, out DateTime lastCheckTime))
            {
                var timeSinceLastCheck = DateTime.UtcNow - lastCheckTime;
                if (timeSinceLastCheck < MinCheckInterval)
                {
                    var remainingSeconds = (int)(MinCheckInterval - timeSinceLastCheck).TotalSeconds;
                    _logger.LogInformation(
                        "HR check request throttled. Last check was {Seconds}s ago. Please wait {Remaining}s",
                        (int)timeSinceLastCheck.TotalSeconds,
                        remainingSeconds);

                    return Ok(new
                    {
                        message = "HR checks already performed recently",
                        lastCheckTime = lastCheckTime,
                        nextCheckAvailableIn = remainingSeconds,
                        throttled = true
                    });
                }
            }

            _logger.LogInformation("Manual HR monitoring check triggered at {Time}", DateTime.UtcNow);

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var notificationPublisher = scope.ServiceProvider.GetRequiredService<INotificationPublisher>();

            var today = DateTime.UtcNow.Date;

            await EndExpiredContractsAsync(context, notificationPublisher, today, cancellationToken);
            await CompleteFinishedLeavesAsync(context, notificationPublisher, today, cancellationToken);
            await NotifyExpiringContractsAsync(context, notificationPublisher, today, cancellationToken);
            await NotifyUpcomingLeavesAsync(context, notificationPublisher, today, cancellationToken);

            // Update cache with current time
            _cache.Set(CACHE_KEY, DateTime.UtcNow, MinCheckInterval);

            _logger.LogInformation("Manual HR monitoring check completed at {Time}", DateTime.UtcNow);

            return Ok(new
            {
                message = "HR monitoring checks completed successfully",
                timestamp = DateTime.UtcNow,
                throttled = false
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during manual HR monitoring check");
            return StatusCode(500, new { message = "Failed to complete HR monitoring checks", error = ex.Message });
        }
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
