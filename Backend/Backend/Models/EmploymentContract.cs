using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class EmploymentContract
{
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public EmploymentType EmploymentType { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal BaseSalary { get; set; }

    public ContractStatus Status { get; set; } = ContractStatus.Active;
}
