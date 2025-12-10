using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Employee
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    public string? AvatarUrl { get; set; }

    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public int? PositionId { get; set; }
    public Position? Position { get; set; }

    // Navigation properties
    public ICollection<EmploymentContract> EmploymentContracts { get; set; } = new List<EmploymentContract>();
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

    // For departments where this employee is the manager
    public ICollection<Department> ManagedDepartments { get; set; } = new List<Department>();
}
