using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api")]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveRequestService _leaveRequestService;

    public LeaveRequestsController(ILeaveRequestService leaveRequestService)
    {
        _leaveRequestService = leaveRequestService;
    }

    [HttpGet("employees/{employeeId}/leave-requests")]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetEmployeeLeaveRequests(int employeeId)
    {
        var leaveRequests = await _leaveRequestService.GetLeaveRequestsByEmployeeIdAsync(employeeId);

        var result = leaveRequests.Select(lr => new LeaveRequestDto
        {
            Id = lr.Id,
            EmployeeId = lr.EmployeeId,
            EmployeeName = lr.Employee?.FullName ?? "",
            Type = lr.Type,
            StartDate = lr.StartDate,
            EndDate = lr.EndDate,
            Reason = lr.Reason,
            Status = lr.Status,
            ApproverName = lr.ApproverName
        }).ToList();

        return Ok(result);
    }

    [HttpGet("leave-requests/{id}")]
    public async Task<ActionResult<LeaveRequestDto>> GetLeaveRequest(int id)
    {
        var leaveRequest = await _leaveRequestService.GetLeaveRequestByIdAsync(id);

        if (leaveRequest == null)
        {
            return NotFound();
        }

        var result = new LeaveRequestDto
        {
            Id = leaveRequest.Id,
            EmployeeId = leaveRequest.EmployeeId,
            EmployeeName = leaveRequest.Employee?.FullName ?? "",
            Type = leaveRequest.Type,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            Reason = leaveRequest.Reason,
            Status = leaveRequest.Status,
            ApproverName = leaveRequest.ApproverName
        };

        return Ok(result);
    }

    [HttpPost("employees/{employeeId}/leave-requests")]
    public async Task<ActionResult<LeaveRequestDto>> CreateLeaveRequest(int employeeId, CreateLeaveRequestDto dto)
    {
        // Ensure employeeId in route matches DTO
        if (dto.EmployeeId != employeeId)
        {
            return BadRequest(new { Message = "EmployeeId mismatch between route and body" });
        }

        var leaveRequest = await _leaveRequestService.CreateLeaveDraftAsync(dto);

        var result = new LeaveRequestDto
        {
            Id = leaveRequest.Id,
            EmployeeId = leaveRequest.EmployeeId,
            EmployeeName = leaveRequest.Employee?.FullName ?? "",
            Type = leaveRequest.Type,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            Reason = leaveRequest.Reason,
            Status = leaveRequest.Status,
            ApproverName = leaveRequest.ApproverName
        };

        return CreatedAtAction(nameof(GetLeaveRequest), new { id = leaveRequest.Id }, result);
    }

    [HttpPut("leave-requests/{id}")]
    public async Task<ActionResult<LeaveRequestDto>> UpdateLeaveRequest(int id, UpdateLeaveRequestDto dto)
    {
        var leaveRequest = await _leaveRequestService.UpdateLeaveDraftAsync(id, dto);

        var result = new LeaveRequestDto
        {
            Id = leaveRequest.Id,
            EmployeeId = leaveRequest.EmployeeId,
            EmployeeName = leaveRequest.Employee?.FullName ?? "",
            Type = leaveRequest.Type,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            Reason = leaveRequest.Reason,
            Status = leaveRequest.Status,
            ApproverName = leaveRequest.ApproverName
        };

        return Ok(result);
    }

    [HttpPost("leave-requests/{id}/submit")]
    public async Task<ActionResult<LeaveRequestDto>> SubmitLeaveRequest(int id)
    {
        var leaveRequest = await _leaveRequestService.SubmitLeaveAsync(id);

        var result = new LeaveRequestDto
        {
            Id = leaveRequest.Id,
            EmployeeId = leaveRequest.EmployeeId,
            EmployeeName = leaveRequest.Employee?.FullName ?? "",
            Type = leaveRequest.Type,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            Reason = leaveRequest.Reason,
            Status = leaveRequest.Status,
            ApproverName = leaveRequest.ApproverName
        };

        return Ok(result);
    }

    [HttpPost("leave-requests/{id}/approve")]
    public async Task<ActionResult<LeaveRequestDto>> ApproveLeaveRequest(int id, [FromBody] ApproveLeaveDto dto)
    {
        var leaveRequest = await _leaveRequestService.ApproveLeaveAsync(id, dto.ApproverName);

        var result = new LeaveRequestDto
        {
            Id = leaveRequest.Id,
            EmployeeId = leaveRequest.EmployeeId,
            EmployeeName = leaveRequest.Employee?.FullName ?? "",
            Type = leaveRequest.Type,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            Reason = leaveRequest.Reason,
            Status = leaveRequest.Status,
            ApproverName = leaveRequest.ApproverName
        };

        return Ok(result);
    }

    [HttpPost("leave-requests/{id}/reject")]
    public async Task<ActionResult<LeaveRequestDto>> RejectLeaveRequest(int id, [FromBody] ApproveLeaveDto dto)
    {
        var leaveRequest = await _leaveRequestService.RejectLeaveAsync(id, dto.ApproverName);

        var result = new LeaveRequestDto
        {
            Id = leaveRequest.Id,
            EmployeeId = leaveRequest.EmployeeId,
            EmployeeName = leaveRequest.Employee?.FullName ?? "",
            Type = leaveRequest.Type,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            Reason = leaveRequest.Reason,
            Status = leaveRequest.Status,
            ApproverName = leaveRequest.ApproverName
        };

        return Ok(result);
    }

    [HttpPost("leave-requests/{id}/cancel")]
    public async Task<ActionResult<LeaveRequestDto>> CancelLeaveRequest(int id)
    {
        var leaveRequest = await _leaveRequestService.CancelLeaveAsync(id);

        var result = new LeaveRequestDto
        {
            Id = leaveRequest.Id,
            EmployeeId = leaveRequest.EmployeeId,
            EmployeeName = leaveRequest.Employee?.FullName ?? "",
            Type = leaveRequest.Type,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            Reason = leaveRequest.Reason,
            Status = leaveRequest.Status,
            ApproverName = leaveRequest.ApproverName
        };

        return Ok(result);
    }

    [HttpDelete("leave-requests/{id}")]
    public async Task<IActionResult> DeleteLeaveRequest(int id)
    {
        var leaveRequest = await _leaveRequestService.GetLeaveRequestByIdAsync(id);
        if (leaveRequest == null)
        {
            return NotFound();
        }

        // Note: In a real application, you might want to add business rules
        // about whether leave requests can be deleted
        return NoContent();
    }
}

// Simple DTO for approve/reject actions
public class ApproveLeaveDto
{
    public string ApproverName { get; set; } = string.Empty;
}
