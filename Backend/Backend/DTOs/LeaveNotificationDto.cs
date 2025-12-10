using Backend.Models;

namespace Backend.DTOs;

public class LeaveNotificationDto
{
    public int LeaveRequestId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeEmail { get; set; } = string.Empty;
    public LeaveType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DaysUntilStart { get; set; }
    public string? Reason { get; set; }
}
