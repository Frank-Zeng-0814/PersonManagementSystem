using Backend.DTOs;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ICloudinaryService _cloudinaryService;

    public EmployeesController(AppDbContext context, ICloudinaryService cloudinaryService)
    {
        _context = context;
        _cloudinaryService = cloudinaryService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmployeeDto>>> GetEmployees()
    {
        var employees = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FullName = e.FullName,
                Email = e.Email,
                Phone = e.Phone,
                AvatarUrl = e.AvatarUrl,
                Status = e.Status,
                CreatedAt = e.CreatedAt,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department != null ? e.Department.Name : null,
                PositionId = e.PositionId,
                PositionTitle = e.Position != null ? e.Position.Title : null
            })
            .ToListAsync();

        return Ok(employees);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Where(e => e.Id == id)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FullName = e.FullName,
                Email = e.Email,
                Phone = e.Phone,
                AvatarUrl = e.AvatarUrl,
                Status = e.Status,
                CreatedAt = e.CreatedAt,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department != null ? e.Department.Name : null,
                PositionId = e.PositionId,
                PositionTitle = e.Position != null ? e.Position.Title : null
            })
            .FirstOrDefaultAsync();

        if (employee == null)
        {
            return NotFound();
        }

        return Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee(CreateEmployeeDto dto)
    {
        // Validate department exists if provided
        if (dto.DepartmentId.HasValue)
        {
            var departmentExists = await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId.Value);
            if (!departmentExists)
            {
                return BadRequest(new { Message = "Department not found" });
            }
        }

        // Validate position exists if provided
        if (dto.PositionId.HasValue)
        {
            var positionExists = await _context.Positions.AnyAsync(p => p.Id == dto.PositionId.Value);
            if (!positionExists)
            {
                return BadRequest(new { Message = "Position not found" });
            }
        }

        var employee = new Employee
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone,
            AvatarUrl = dto.AvatarUrl,
            DepartmentId = dto.DepartmentId,
            PositionId = dto.PositionId,
            Status = EmployeeStatus.Active
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        var result = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Where(e => e.Id == employee.Id)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FullName = e.FullName,
                Email = e.Email,
                Phone = e.Phone,
                AvatarUrl = e.AvatarUrl,
                Status = e.Status,
                CreatedAt = e.CreatedAt,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department != null ? e.Department.Name : null,
                PositionId = e.PositionId,
                PositionTitle = e.Position != null ? e.Position.Title : null
            })
            .FirstAsync();

        return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<EmployeeDto>> UpdateEmployee(int id, CreateEmployeeDto dto)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        // Validate department exists if provided
        if (dto.DepartmentId.HasValue)
        {
            var departmentExists = await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId.Value);
            if (!departmentExists)
            {
                return BadRequest(new { Message = "Department not found" });
            }
        }

        // Validate position exists if provided
        if (dto.PositionId.HasValue)
        {
            var positionExists = await _context.Positions.AnyAsync(p => p.Id == dto.PositionId.Value);
            if (!positionExists)
            {
                return BadRequest(new { Message = "Position not found" });
            }
        }

        employee.FullName = dto.FullName;
        employee.Email = dto.Email;
        employee.Phone = dto.Phone;
        employee.AvatarUrl = dto.AvatarUrl;
        employee.DepartmentId = dto.DepartmentId;
        employee.PositionId = dto.PositionId;

        await _context.SaveChangesAsync();

        var result = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Where(e => e.Id == employee.Id)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FullName = e.FullName,
                Email = e.Email,
                Phone = e.Phone,
                AvatarUrl = e.AvatarUrl,
                Status = e.Status,
                CreatedAt = e.CreatedAt,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department != null ? e.Department.Name : null,
                PositionId = e.PositionId,
                PositionTitle = e.Position != null ? e.Position.Title : null
            })
            .FirstAsync();

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/upload-avatar")]
    public async Task<ActionResult<object>> UploadAvatar(int id, IFormFile file)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        try
        {
            var avatarUrl = await _cloudinaryService.UploadImageAsync(file);
            employee.AvatarUrl = avatarUrl;
            await _context.SaveChangesAsync();

            return Ok(new { avatarUrl });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("{id}/set-active")]
    public async Task<IActionResult> SetActive(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        employee.Status = EmployeeStatus.Active;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/set-on-leave")]
    public async Task<IActionResult> SetOnLeave(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        employee.Status = EmployeeStatus.OnLeave;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
