using System.ComponentModel.DataAnnotations;
using Backend.Models;

namespace Backend.DTOs;

public class CreateContractDto
{
    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required]
    public EmploymentType EmploymentType { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "BaseSalary must be positive")]
    public decimal BaseSalary { get; set; }
}
