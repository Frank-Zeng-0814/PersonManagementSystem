using Backend.DTOs;
using Backend.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Services;

/// <summary>
/// SignalR-based implementation of INotificationPublisher.
/// Broadcasts all notifications to all connected clients via WebSocket.
/// </summary>
public class SignalRNotificationPublisher : INotificationPublisher
{
    private readonly IHubContext<NotificationsHub> _hubContext;
    private readonly ILogger<SignalRNotificationPublisher> _logger;

    public SignalRNotificationPublisher(
        IHubContext<NotificationsHub> hubContext,
        ILogger<SignalRNotificationPublisher> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task PublishContractExpiringAsync(ContractNotificationDto notification, CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync(
                "ContractExpiringSoon",
                notification,
                cancellationToken);

            _logger.LogInformation(
                "Published ContractExpiringSoon notification for Employee {EmployeeName} (Contract ID: {ContractId})",
                notification.EmployeeName,
                notification.ContractId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish ContractExpiringSoon notification");
        }
    }

    public async Task PublishUpcomingLeaveAsync(LeaveNotificationDto notification, CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync(
                "UpcomingLeave",
                notification,
                cancellationToken);

            _logger.LogInformation(
                "Published UpcomingLeave notification for Employee {EmployeeName} (Leave ID: {LeaveId})",
                notification.EmployeeName,
                notification.LeaveRequestId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish UpcomingLeave notification");
        }
    }

    public async Task PublishContractEndedAsync(ContractNotificationDto notification, CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync(
                "ContractUpdated",
                new
                {
                    ContractId = notification.ContractId,
                    EmployeeId = notification.EmployeeId,
                    EmployeeName = notification.EmployeeName,
                    Status = "Ended",
                    EndDate = notification.EndDate,
                    Message = "Employment contract has ended"
                },
                cancellationToken);

            _logger.LogInformation(
                "Published ContractUpdated (Ended) notification for Employee {EmployeeName} (Contract ID: {ContractId})",
                notification.EmployeeName,
                notification.ContractId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish ContractUpdated notification");
        }
    }

    public async Task PublishLeaveCompletedAsync(LeaveNotificationDto notification, CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync(
                "LeaveRequestUpdated",
                new
                {
                    LeaveRequestId = notification.LeaveRequestId,
                    EmployeeId = notification.EmployeeId,
                    EmployeeName = notification.EmployeeName,
                    Status = "Completed",
                    StartDate = notification.StartDate,
                    EndDate = notification.EndDate,
                    LeaveType = notification.Type,
                    Message = "Leave request has been completed"
                },
                cancellationToken);

            _logger.LogInformation(
                "Published LeaveRequestUpdated (Completed) notification for Employee {EmployeeName} (Leave ID: {LeaveId})",
                notification.EmployeeName,
                notification.LeaveRequestId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish LeaveRequestUpdated notification");
        }
    }

    /// <summary>
    /// Publishes a LeaveRequestUpdated event when leave status changes (Approved, Rejected, etc.)
    /// </summary>
    public async Task PublishLeaveRequestUpdatedAsync(int leaveRequestId, int employeeId, string employeeName, string status, string? message = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync(
                "LeaveRequestUpdated",
                new
                {
                    LeaveRequestId = leaveRequestId,
                    EmployeeId = employeeId,
                    EmployeeName = employeeName,
                    Status = status,
                    Message = message ?? $"Leave request status changed to {status}"
                },
                cancellationToken);

            _logger.LogInformation(
                "Published LeaveRequestUpdated notification for Employee {EmployeeName} (Leave ID: {LeaveId}, Status: {Status})",
                employeeName,
                leaveRequestId,
                status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish LeaveRequestUpdated notification");
        }
    }

    /// <summary>
    /// Publishes a ContractUpdated event when contract status changes (Active, Ended)
    /// </summary>
    public async Task PublishContractUpdatedAsync(int contractId, int employeeId, string employeeName, string status, DateTime? endDate = null, string? message = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync(
                "ContractUpdated",
                new
                {
                    ContractId = contractId,
                    EmployeeId = employeeId,
                    EmployeeName = employeeName,
                    Status = status,
                    EndDate = endDate,
                    Message = message ?? $"Contract status changed to {status}"
                },
                cancellationToken);

            _logger.LogInformation(
                "Published ContractUpdated notification for Employee {EmployeeName} (Contract ID: {ContractId}, Status: {Status})",
                employeeName,
                contractId,
                status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish ContractUpdated notification");
        }
    }

    /// <summary>
    /// Publishes an EmployeeUpdated event when employee data changes
    /// </summary>
    public async Task PublishEmployeeUpdatedAsync(int employeeId, string employeeName, string changeType, string? message = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync(
                "EmployeeUpdated",
                new
                {
                    EmployeeId = employeeId,
                    EmployeeName = employeeName,
                    ChangeType = changeType,
                    Message = message ?? $"Employee {changeType}"
                },
                cancellationToken);

            _logger.LogInformation(
                "Published EmployeeUpdated notification for Employee {EmployeeName} (ID: {EmployeeId}, Change: {ChangeType})",
                employeeName,
                employeeId,
                changeType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish EmployeeUpdated notification");
        }
    }
}
