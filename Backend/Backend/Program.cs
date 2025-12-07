using Backend.Models;
using Backend.Services;
using Backend.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var MyAllowSpecificOrigins = "_myAllowSpecific Origins";

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

var allowedOrigins = builder.Configuration.GetValue<string>("AllowedOrigins") ?? "http://localhost:5173";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins(allowedOrigins.Split(','))
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                      });
});

builder.Services.AddControllers();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Use PostgreSQL if connection string exists, otherwise use SQLite
var postgresConnection = builder.Configuration.GetConnectionString("PostgreSQL");
if (!string.IsNullOrEmpty(postgresConnection))
{
    builder.Services.AddDbContext<AppDbContext>(op => op.UseNpgsql(postgresConnection));
}
else
{
    string sqliteConnection = builder.Configuration.GetConnectionString("Default") ?? throw new ArgumentNullException("connectionString is null");
    builder.Services.AddDbContext<AppDbContext>(op => op.UseSqlite(sqliteConnection));
}

var app = builder.Build();

// Auto migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSerilogRequestLogging();

app.UseCors(MyAllowSpecificOrigins);
app.MapControllers();

app.Run();
