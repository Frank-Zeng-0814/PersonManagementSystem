using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api")]
public class EmploymentContractsController : ControllerBase
{
    private readonly IEmploymentContractService _contractService;

    public EmploymentContractsController(IEmploymentContractService contractService)
    {
        _contractService = contractService;
    }

    [HttpGet("employees/{employeeId}/contracts")]
    public async Task<ActionResult<List<ContractDto>>> GetEmployeeContracts(int employeeId)
    {
        var contracts = await _contractService.GetActiveContractsByEmployeeIdAsync(employeeId);

        var result = contracts.Select(c => new ContractDto
        {
            Id = c.Id,
            EmployeeId = c.EmployeeId,
            EmployeeName = c.Employee?.FullName ?? "",
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            EmploymentType = c.EmploymentType,
            BaseSalary = c.BaseSalary,
            Status = c.Status
        }).ToList();

        return Ok(result);
    }

    [HttpGet("contracts/{id}")]
    public async Task<ActionResult<ContractDto>> GetContract(int id)
    {
        var contract = await _contractService.GetContractByIdAsync(id);

        if (contract == null)
        {
            return NotFound();
        }

        var result = new ContractDto
        {
            Id = contract.Id,
            EmployeeId = contract.EmployeeId,
            EmployeeName = contract.Employee?.FullName ?? "",
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            EmploymentType = contract.EmploymentType,
            BaseSalary = contract.BaseSalary,
            Status = contract.Status
        };

        return Ok(result);
    }

    [HttpPost("employees/{employeeId}/contracts")]
    public async Task<ActionResult<ContractDto>> CreateContract(int employeeId, CreateContractDto dto)
    {
        // Ensure employeeId in route matches DTO
        if (dto.EmployeeId != employeeId)
        {
            return BadRequest(new { Message = "EmployeeId mismatch between route and body" });
        }

        var contract = await _contractService.CreateContractAsync(dto);

        var result = new ContractDto
        {
            Id = contract.Id,
            EmployeeId = contract.EmployeeId,
            EmployeeName = contract.Employee?.FullName ?? "",
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            EmploymentType = contract.EmploymentType,
            BaseSalary = contract.BaseSalary,
            Status = contract.Status
        };

        return CreatedAtAction(nameof(GetContract), new { id = contract.Id }, result);
    }

    [HttpDelete("contracts/{id}")]
    public async Task<IActionResult> DeleteContract(int id)
    {
        var contract = await _contractService.GetContractByIdAsync(id);
        if (contract == null)
        {
            return NotFound();
        }

        // Note: In a real application, you might want to add business rules
        // about whether contracts can be deleted
        return NoContent();
    }
}
