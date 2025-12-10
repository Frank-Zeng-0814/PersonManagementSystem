using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class CreatePositionDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public int DepartmentId { get; set; }
}
