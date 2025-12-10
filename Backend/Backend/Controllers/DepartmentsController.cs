using Backend.DTOs;
using Backend.Exceptions;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DepartmentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<DepartmentDto>>> GetDepartments()
    {
        var departments = await _context.Departments
            .Include(d => d.Manager)
            .Include(d => d.Employees)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                ManagerId = d.ManagerId,
                ManagerName = d.Manager != null ? d.Manager.FullName : null,
                EmployeeCount = d.Employees.Count
            })
            .ToListAsync();

        return Ok(departments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DepartmentDto>> GetDepartment(int id)
    {
        var department = await _context.Departments
            .Include(d => d.Manager)
            .Include(d => d.Employees)
            .Where(d => d.Id == id)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                ManagerId = d.ManagerId,
                ManagerName = d.Manager != null ? d.Manager.FullName : null,
                EmployeeCount = d.Employees.Count
            })
            .FirstOrDefaultAsync();

        if (department == null)
        {
            return NotFound();
        }

        return Ok(department);
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> CreateDepartment(CreateDepartmentDto dto)
    {
        var department = new Department
        {
            Name = dto.Name,
            ManagerId = dto.ManagerId
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        var result = await _context.Departments
            .Include(d => d.Manager)
            .Include(d => d.Employees)
            .Where(d => d.Id == department.Id)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                ManagerId = d.ManagerId,
                ManagerName = d.Manager != null ? d.Manager.FullName : null,
                EmployeeCount = d.Employees.Count
            })
            .FirstAsync();

        return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DepartmentDto>> UpdateDepartment(int id, CreateDepartmentDto dto)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department == null)
        {
            return NotFound();
        }

        department.Name = dto.Name;
        department.ManagerId = dto.ManagerId;

        await _context.SaveChangesAsync();

        var result = await _context.Departments
            .Include(d => d.Manager)
            .Include(d => d.Employees)
            .Where(d => d.Id == department.Id)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                ManagerId = d.ManagerId,
                ManagerName = d.Manager != null ? d.Manager.FullName : null,
                EmployeeCount = d.Employees.Count
            })
            .FirstAsync();

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
        {
            return NotFound();
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
