using Backend.DTOs;
using Backend.Exceptions;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PositionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PositionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<PositionDto>>> GetPositions()
    {
        var positions = await _context.Positions
            .Include(p => p.Department)
            .Include(p => p.Employees)
            .Select(p => new PositionDto
            {
                Id = p.Id,
                Title = p.Title,
                DepartmentId = p.DepartmentId,
                DepartmentName = p.Department.Name,
                EmployeeCount = p.Employees.Count
            })
            .ToListAsync();

        return Ok(positions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PositionDto>> GetPosition(int id)
    {
        var position = await _context.Positions
            .Include(p => p.Department)
            .Include(p => p.Employees)
            .Where(p => p.Id == id)
            .Select(p => new PositionDto
            {
                Id = p.Id,
                Title = p.Title,
                DepartmentId = p.DepartmentId,
                DepartmentName = p.Department.Name,
                EmployeeCount = p.Employees.Count
            })
            .FirstOrDefaultAsync();

        if (position == null)
        {
            return NotFound();
        }

        return Ok(position);
    }

    [HttpPost]
    public async Task<ActionResult<PositionDto>> CreatePosition(CreatePositionDto dto)
    {
        // Validate department exists
        var departmentExists = await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId);
        if (!departmentExists)
        {
            throw new DomainException("Department not found", "DEPARTMENT_NOT_FOUND");
        }

        var position = new Position
        {
            Title = dto.Title,
            DepartmentId = dto.DepartmentId
        };

        _context.Positions.Add(position);
        await _context.SaveChangesAsync();

        var result = await _context.Positions
            .Include(p => p.Department)
            .Include(p => p.Employees)
            .Where(p => p.Id == position.Id)
            .Select(p => new PositionDto
            {
                Id = p.Id,
                Title = p.Title,
                DepartmentId = p.DepartmentId,
                DepartmentName = p.Department.Name,
                EmployeeCount = p.Employees.Count
            })
            .FirstAsync();

        return CreatedAtAction(nameof(GetPosition), new { id = position.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PositionDto>> UpdatePosition(int id, CreatePositionDto dto)
    {
        var position = await _context.Positions.FindAsync(id);
        if (position == null)
        {
            return NotFound();
        }

        // Validate department exists
        var departmentExists = await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId);
        if (!departmentExists)
        {
            throw new DomainException("Department not found", "DEPARTMENT_NOT_FOUND");
        }

        position.Title = dto.Title;
        position.DepartmentId = dto.DepartmentId;

        await _context.SaveChangesAsync();

        var result = await _context.Positions
            .Include(p => p.Department)
            .Include(p => p.Employees)
            .Where(p => p.Id == position.Id)
            .Select(p => new PositionDto
            {
                Id = p.Id,
                Title = p.Title,
                DepartmentId = p.DepartmentId,
                DepartmentName = p.Department.Name,
                EmployeeCount = p.Employees.Count
            })
            .FirstAsync();

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePosition(int id)
    {
        var position = await _context.Positions.FindAsync(id);
        if (position == null)
        {
            return NotFound();
        }

        _context.Positions.Remove(position);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
