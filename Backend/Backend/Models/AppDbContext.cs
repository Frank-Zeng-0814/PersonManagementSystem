using Microsoft.EntityFrameworkCore;

namespace Backend.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<EmploymentContract> EmploymentContracts { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Employee configuration
        modelBuilder.Entity<Employee>(entity =>
        {
            // Default value for CreatedAt
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Unique constraint on Email
            entity.HasIndex(e => e.Email)
                .IsUnique();

            // Self-referencing relationship for Department.Manager
            // Department -> Employee (Manager)
            entity.HasMany(e => e.ManagedDepartments)
                .WithOne(d => d.Manager)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Employee -> Department
            entity.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Employee -> Position
            entity.HasOne(e => e.Position)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Department configuration
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasIndex(d => d.Name);
        });

        // Position configuration
        modelBuilder.Entity<Position>(entity =>
        {
            // Position -> Department (required)
            entity.HasOne(p => p.Department)
                .WithMany(d => d.Positions)
                .HasForeignKey(p => p.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(p => new { p.DepartmentId, p.Title });
        });

        // EmploymentContract configuration
        modelBuilder.Entity<EmploymentContract>(entity =>
        {
            entity.HasOne(ec => ec.Employee)
                .WithMany(e => e.EmploymentContracts)
                .HasForeignKey(ec => ec.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(ec => new { ec.EmployeeId, ec.Status });
        });

        // LeaveRequest configuration
        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasOne(lr => lr.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(lr => lr.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(lr => new { lr.EmployeeId, lr.Status });
            entity.HasIndex(lr => lr.StartDate);
        });
    }
}
