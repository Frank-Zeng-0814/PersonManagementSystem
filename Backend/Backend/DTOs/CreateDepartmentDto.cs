using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class CreateDepartmentDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int? ManagerId { get; set; }
}
