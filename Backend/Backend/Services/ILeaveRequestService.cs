using Backend.DTOs;
using Backend.Models;

namespace Backend.Services;

public interface ILeaveRequestService
{
    Task<LeaveRequest> CreateLeaveDraftAsync(CreateLeaveRequestDto dto, CancellationToken cancellationToken = default);
    Task<LeaveRequest> UpdateLeaveDraftAsync(int leaveRequestId, UpdateLeaveRequestDto dto, CancellationToken cancellationToken = default);
    Task<LeaveRequest> SubmitLeaveAsync(int leaveRequestId, CancellationToken cancellationToken = default);
    Task<LeaveRequest> ApproveLeaveAsync(int leaveRequestId, string approverName, CancellationToken cancellationToken = default);
    Task<LeaveRequest> RejectLeaveAsync(int leaveRequestId, string approverName, CancellationToken cancellationToken = default);
    Task<LeaveRequest> CancelLeaveAsync(int leaveRequestId, CancellationToken cancellationToken = default);
    Task CompleteExpiredLeavesAsync(CancellationToken cancellationToken = default);
    Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
}
