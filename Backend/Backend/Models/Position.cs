using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Position
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    // Navigation properties
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
