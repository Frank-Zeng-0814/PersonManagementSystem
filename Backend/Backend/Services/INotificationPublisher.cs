using Backend.DTOs;

namespace Backend.Services;
/// Interface for publishing HR-related notifications.
/// This will be implemented later with SignalR for real-time notifications
public interface INotificationPublisher
{
    /// Publishes a notification about a contract that is expiring soon.
    Task PublishContractExpiringAsync(ContractNotificationDto notification, CancellationToken cancellationToken = default);

    /// Publishes a notification about an upcoming leave request.
    Task PublishUpcomingLeaveAsync(LeaveNotificationDto notification, CancellationToken cancellationToken = default);

    /// Publishes a notification about a contract that has ended.
    Task PublishContractEndedAsync(ContractNotificationDto notification, CancellationToken cancellationToken = default);

    /// Publishes a notification about a leave that has been completed.
    Task PublishLeaveCompletedAsync(LeaveNotificationDto notification, CancellationToken cancellationToken = default);

    /// Publishes a notification when leave request status changes (Submit, Approve, Reject, Cancel).
    Task PublishLeaveRequestUpdatedAsync(int leaveRequestId, int employeeId, string employeeName, string status, string? message = null, CancellationToken cancellationToken = default);

    /// Publishes a notification when contract status changes.
    Task PublishContractUpdatedAsync(int contractId, int employeeId, string employeeName, string status, DateTime? endDate = null, string? message = null, CancellationToken cancellationToken = default);

    /// Publishes a notification when employee data changes.
    Task PublishEmployeeUpdatedAsync(int employeeId, string employeeName, string changeType, string? message = null, CancellationToken cancellationToken = default);
}
