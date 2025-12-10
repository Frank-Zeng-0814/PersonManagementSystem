using Backend.DTOs;

namespace Backend.Services;

/// <summary>
/// Default implementation that logs notifications.
/// This will be replaced with SignalR implementation later.
/// </summary>
public class LoggingNotificationPublisher : INotificationPublisher
{
    private readonly ILogger<LoggingNotificationPublisher> _logger;

    public LoggingNotificationPublisher(ILogger<LoggingNotificationPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishContractExpiringAsync(ContractNotificationDto notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Contract expiring soon: Employee {EmployeeName} (ID: {EmployeeId}), Contract ID: {ContractId}, Expires in {Days} days on {EndDate}",
            notification.EmployeeName,
            notification.EmployeeId,
            notification.ContractId,
            notification.DaysUntilExpiry,
            notification.EndDate);

        return Task.CompletedTask;
    }

    public Task PublishUpcomingLeaveAsync(LeaveNotificationDto notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Upcoming leave: Employee {EmployeeName} (ID: {EmployeeId}), Leave ID: {LeaveId}, Starts in {Days} days on {StartDate}, Type: {Type}",
            notification.EmployeeName,
            notification.EmployeeId,
            notification.LeaveRequestId,
            notification.DaysUntilStart,
            notification.StartDate,
            notification.Type);

        return Task.CompletedTask;
    }

    public Task PublishContractEndedAsync(ContractNotificationDto notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Contract ended: Employee {EmployeeName} (ID: {EmployeeId}), Contract ID: {ContractId}",
            notification.EmployeeName,
            notification.EmployeeId,
            notification.ContractId);

        return Task.CompletedTask;
    }

    public Task PublishLeaveCompletedAsync(LeaveNotificationDto notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Leave completed: Employee {EmployeeName} (ID: {EmployeeId}), Leave ID: {LeaveId}",
            notification.EmployeeName,
            notification.EmployeeId,
            notification.LeaveRequestId);

        return Task.CompletedTask;
    }
}
