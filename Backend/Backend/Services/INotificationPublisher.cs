using Backend.DTOs;

namespace Backend.Services;

/// <summary>
/// Interface for publishing HR-related notifications.
/// This will be implemented later with SignalR for real-time notifications.
/// </summary>
public interface INotificationPublisher
{
    /// <summary>
    /// Publishes a notification about a contract that is expiring soon.
    /// </summary>
    Task PublishContractExpiringAsync(ContractNotificationDto notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a notification about an upcoming leave request.
    /// </summary>
    Task PublishUpcomingLeaveAsync(LeaveNotificationDto notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a notification about a contract that has ended.
    /// </summary>
    Task PublishContractEndedAsync(ContractNotificationDto notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a notification about a leave that has been completed.
    /// </summary>
    Task PublishLeaveCompletedAsync(LeaveNotificationDto notification, CancellationToken cancellationToken = default);
}
