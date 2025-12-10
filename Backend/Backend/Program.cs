using Backend.Models;
using Backend.Services;
using Backend.Middleware;
using Backend.Hubs;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning)
    .CreateLogger();

var MyAllowSpecificOrigins = "_myAllowSpecific Origins";

var builder = WebApplication.CreateBuilder(args);

// Configure port for Railway deployment
var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";
builder.WebHost.UseUrls($"http://*:{port}");

builder.Host.UseSerilog();

var allowedOrigins = builder.Configuration.GetValue<string>("AllowedOrigins") ?? "http://localhost:5173";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins(allowedOrigins.Split(','))
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials(); // Required for SignalR
                      });
});

builder.Services.AddControllers();
builder.Services.AddSignalR(); // Add SignalR services
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IEmploymentContractService, EmploymentContractService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddSingleton<INotificationPublisher, SignalRNotificationPublisher>(); // Use SignalR implementation
builder.Services.AddHostedService<HrMonitoringService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var postgresConnection = builder.Configuration.GetConnectionString("PostgreSQL")
    ?? throw new InvalidOperationException("PostgreSQL connection string is required.");
builder.Services.AddDbContext<AppDbContext>(op => op.UseNpgsql(postgresConnection));

var app = builder.Build();

// Auto-apply pending migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        var pendingMigrations = db.Database.GetPendingMigrations();
        if (pendingMigrations.Any())
        {
            Log.Information("Applying {Count} pending migrations: {Migrations}",
                pendingMigrations.Count(), string.Join(", ", pendingMigrations));
            db.Database.Migrate();
            Log.Information("Database migration completed successfully");
        }
        else
        {
            Log.Information("No pending migrations - database is up to date");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Database migration failed");
        throw;
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSerilogRequestLogging();

app.UseCors(MyAllowSpecificOrigins);
app.MapControllers();
app.MapHub<NotificationsHub>("/hubs/notifications"); // Map SignalR Hub

app.Run();
