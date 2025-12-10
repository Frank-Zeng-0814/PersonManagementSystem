using Backend.DTOs;
using Backend.Exceptions;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class LeaveRequestService : ILeaveRequestService
{
    private readonly AppDbContext _context;
    private readonly INotificationPublisher _notificationPublisher;

    public LeaveRequestService(
        AppDbContext context,
        INotificationPublisher notificationPublisher)
    {
        _context = context;
        _notificationPublisher = notificationPublisher;
    }

    public async Task<LeaveRequest> CreateLeaveDraftAsync(CreateLeaveRequestDto dto, CancellationToken cancellationToken = default)
    {
        // Validate employee exists
        var employee = await _context.Employees.FindAsync(new object[] { dto.EmployeeId }, cancellationToken);
        if (employee == null)
        {
            throw new DomainException($"Employee with ID {dto.EmployeeId} not found", "EMPLOYEE_NOT_FOUND");
        }

        // Validate dates
        if (dto.EndDate < dto.StartDate)
        {
            throw new DomainException("EndDate must be after or equal to StartDate", "INVALID_DATE_RANGE");
        }

        // Validate dates fall within an active contract
        await ValidateLeaveDatesWithinActiveContractAsync(dto.EmployeeId, dto.StartDate, dto.EndDate, cancellationToken);

        // Create leave request in Draft status
        var leaveRequest = new LeaveRequest
        {
            EmployeeId = dto.EmployeeId,
            Type = dto.Type,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Reason = dto.Reason,
            Status = LeaveRequestStatus.Draft
        };

        _context.LeaveRequests.Add(leaveRequest);
        await _context.SaveChangesAsync(cancellationToken);

        return leaveRequest;
    }

    public async Task<LeaveRequest> UpdateLeaveDraftAsync(int leaveRequestId, UpdateLeaveRequestDto dto, CancellationToken cancellationToken = default)
    {
        var leaveRequest = await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .FirstOrDefaultAsync(lr => lr.Id == leaveRequestId, cancellationToken);

        if (leaveRequest == null)
        {
            throw new DomainException($"Leave request with ID {leaveRequestId} not found", "LEAVE_REQUEST_NOT_FOUND");
        }

        // Only Draft status can be edited
        if (leaveRequest.Status != LeaveRequestStatus.Draft)
        {
            throw new DomainException("Only leave requests in Draft status can be edited", "INVALID_STATUS_TRANSITION");
        }

        // Update fields
        if (dto.Type.HasValue)
        {
            leaveRequest.Type = dto.Type.Value;
        }

        if (dto.StartDate.HasValue)
        {
            leaveRequest.StartDate = dto.StartDate.Value;
        }

        if (dto.EndDate.HasValue)
        {
            leaveRequest.EndDate = dto.EndDate.Value;
        }

        if (dto.Reason != null)
        {
            leaveRequest.Reason = dto.Reason;
        }

        // Validate updated dates
        if (leaveRequest.EndDate < leaveRequest.StartDate)
        {
            throw new DomainException("EndDate must be after or equal to StartDate", "INVALID_DATE_RANGE");
        }

        // Validate dates fall within an active contract
        await ValidateLeaveDatesWithinActiveContractAsync(
            leaveRequest.EmployeeId,
            leaveRequest.StartDate,
            leaveRequest.EndDate,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return leaveRequest;
    }

    public async Task<LeaveRequest> SubmitLeaveAsync(int leaveRequestId, CancellationToken cancellationToken = default)
    {
        var leaveRequest = await _context.LeaveRequests.FindAsync(new object[] { leaveRequestId }, cancellationToken);

        if (leaveRequest == null)
        {
            throw new DomainException($"Leave request with ID {leaveRequestId} not found", "LEAVE_REQUEST_NOT_FOUND");
        }

        // Only Draft can be submitted
        if (leaveRequest.Status != LeaveRequestStatus.Draft)
        {
            throw new DomainException("Only leave requests in Draft status can be submitted", "INVALID_STATUS_TRANSITION");
        }

        leaveRequest.Status = LeaveRequestStatus.Submitted;
        await _context.SaveChangesAsync(cancellationToken);

        return leaveRequest;
    }

    public async Task<LeaveRequest> ApproveLeaveAsync(int leaveRequestId, string approverName, CancellationToken cancellationToken = default)
    {
        var leaveRequest = await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .FirstOrDefaultAsync(lr => lr.Id == leaveRequestId, cancellationToken);

        if (leaveRequest == null)
        {
            throw new DomainException($"Leave request with ID {leaveRequestId} not found", "LEAVE_REQUEST_NOT_FOUND");
        }

        // Only Submitted can be approved
        if (leaveRequest.Status != LeaveRequestStatus.Submitted)
        {
            throw new DomainException("Only leave requests in Submitted status can be approved", "INVALID_STATUS_TRANSITION");
        }

        // Check for overlapping approved leaves
        var hasOverlap = await _context.LeaveRequests
            .Where(lr => lr.EmployeeId == leaveRequest.EmployeeId
                && lr.Id != leaveRequestId
                && lr.Status == LeaveRequestStatus.Approved)
            .AnyAsync(lr =>
                (leaveRequest.StartDate >= lr.StartDate && leaveRequest.StartDate <= lr.EndDate) ||
                (leaveRequest.EndDate >= lr.StartDate && leaveRequest.EndDate <= lr.EndDate) ||
                (leaveRequest.StartDate <= lr.StartDate && leaveRequest.EndDate >= lr.EndDate),
                cancellationToken);

        if (hasOverlap)
        {
            throw new DomainException(
                "Employee already has an approved leave request that overlaps with this date range",
                "OVERLAPPING_LEAVE");
        }

        leaveRequest.Status = LeaveRequestStatus.Approved;
        leaveRequest.ApproverName = approverName;
        await _context.SaveChangesAsync(cancellationToken);

        // Publish notification via SignalR
        if (_notificationPublisher is SignalRNotificationPublisher signalRPublisher)
        {
            await signalRPublisher.PublishLeaveRequestUpdatedAsync(
                leaveRequest.Id,
                leaveRequest.EmployeeId,
                leaveRequest.Employee.FullName,
                "Approved",
                $"Leave request approved by {approverName}",
                cancellationToken);
        }

        return leaveRequest;
    }

    public async Task<LeaveRequest> RejectLeaveAsync(int leaveRequestId, string approverName, CancellationToken cancellationToken = default)
    {
        var leaveRequest = await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .FirstOrDefaultAsync(lr => lr.Id == leaveRequestId, cancellationToken);

        if (leaveRequest == null)
        {
            throw new DomainException($"Leave request with ID {leaveRequestId} not found", "LEAVE_REQUEST_NOT_FOUND");
        }

        // Only Submitted can be rejected
        if (leaveRequest.Status != LeaveRequestStatus.Submitted)
        {
            throw new DomainException("Only leave requests in Submitted status can be rejected", "INVALID_STATUS_TRANSITION");
        }

        leaveRequest.Status = LeaveRequestStatus.Rejected;
        leaveRequest.ApproverName = approverName;
        await _context.SaveChangesAsync(cancellationToken);

        // Publish notification via SignalR
        if (_notificationPublisher is SignalRNotificationPublisher signalRPublisher)
        {
            await signalRPublisher.PublishLeaveRequestUpdatedAsync(
                leaveRequest.Id,
                leaveRequest.EmployeeId,
                leaveRequest.Employee.FullName,
                "Rejected",
                $"Leave request rejected by {approverName}",
                cancellationToken);
        }

        return leaveRequest;
    }

    public async Task<LeaveRequest> CancelLeaveAsync(int leaveRequestId, CancellationToken cancellationToken = default)
    {
        var leaveRequest = await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .FirstOrDefaultAsync(lr => lr.Id == leaveRequestId, cancellationToken);

        if (leaveRequest == null)
        {
            throw new DomainException($"Leave request with ID {leaveRequestId} not found", "LEAVE_REQUEST_NOT_FOUND");
        }

        // Only Submitted can be cancelled
        if (leaveRequest.Status != LeaveRequestStatus.Submitted)
        {
            throw new DomainException("Only leave requests in Submitted status can be cancelled", "INVALID_STATUS_TRANSITION");
        }

        leaveRequest.Status = LeaveRequestStatus.Cancelled;
        await _context.SaveChangesAsync(cancellationToken);

        // Publish notification via SignalR
        if (_notificationPublisher is SignalRNotificationPublisher signalRPublisher)
        {
            await signalRPublisher.PublishLeaveRequestUpdatedAsync(
                leaveRequest.Id,
                leaveRequest.EmployeeId,
                leaveRequest.Employee.FullName,
                "Cancelled",
                "Leave request has been cancelled",
                cancellationToken);
        }

        return leaveRequest;
    }

    public async Task CompleteExpiredLeavesAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;

        // Find all approved leaves that have ended
        var expiredLeaves = await _context.LeaveRequests
            .Where(lr => lr.Status == LeaveRequestStatus.Approved && lr.EndDate < today)
            .ToListAsync(cancellationToken);

        foreach (var leave in expiredLeaves)
        {
            leave.Status = LeaveRequestStatus.Completed;
        }

        if (expiredLeaves.Any())
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .FirstOrDefaultAsync(lr => lr.Id == id, cancellationToken);
    }

    public async Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.LeaveRequests
            .Where(lr => lr.EmployeeId == employeeId)
            .OrderByDescending(lr => lr.StartDate)
            .ToListAsync(cancellationToken);
    }

    private async Task ValidateLeaveDatesWithinActiveContractAsync(
        int employeeId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        // Check if the leave dates fall within an active employment contract
        var hasValidContract = await _context.EmploymentContracts
            .Where(c => c.EmployeeId == employeeId && c.Status == ContractStatus.Active)
            .AnyAsync(c =>
                startDate >= c.StartDate &&
                endDate <= (c.EndDate ?? DateTime.MaxValue),
                cancellationToken);

        if (!hasValidContract)
        {
            throw new DomainException(
                "Leave request dates must fall within an active employment contract period",
                "NO_VALID_CONTRACT");
        }
    }
}
