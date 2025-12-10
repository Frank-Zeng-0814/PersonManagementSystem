using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class CreateEmployeeDto
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    public string? AvatarUrl { get; set; }

    public int? DepartmentId { get; set; }

    public int? PositionId { get; set; }
}
