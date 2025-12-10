using Backend.Models;

namespace Backend.DTOs;

public class ContractDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public decimal BaseSalary { get; set; }
    public ContractStatus Status { get; set; }
}
