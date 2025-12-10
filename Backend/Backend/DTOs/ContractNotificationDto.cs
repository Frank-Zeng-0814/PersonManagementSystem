using Backend.Models;

namespace Backend.DTOs;

public class ContractNotificationDto
{
    public int ContractId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeEmail { get; set; } = string.Empty;
    public DateTime EndDate { get; set; }
    public int DaysUntilExpiry { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public decimal BaseSalary { get; set; }
}
