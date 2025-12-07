using Microsoft.EntityFrameworkCore;

namespace Backend.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Person> People { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Use different SQL for SQLite vs PostgreSQL
        var defaultSql = Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite"
            ? "datetime('now')"
            : "CURRENT_TIMESTAMP";

        modelBuilder.Entity<Person>()
            .Property(p => p.CreatedAt)
            .HasDefaultValueSql(defaultSql);
    }
}
