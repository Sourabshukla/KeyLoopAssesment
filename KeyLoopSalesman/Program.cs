using KeyLoopSalesman.Api.Middleware;
using KeyLoopSalesman.Application.Interfaces;
using KeyLoopSalesman.Application.Services;
using KeyLoopSalesman.Infrastructure.Data;
using KeyLoopSalesman.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// API controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Centralized error handling
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// SQL Server LocalDB
builder.Services.AddDbContext<LeadManagementDbContext>(
    options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("LeadManagementDb") ?? throw new InvalidOperationException("Database connection string is missing.");
        options.UseSqlServer(connectionString);
    });

// Application and Infrastructure services
builder.Services.AddScoped<ILeadService, LeadService>();
builder.Services.AddScoped<ILeadRepository, LeadRepository>();

var app = builder.Build();

// Development-only Swagger and seed data
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider.GetRequiredService<LeadManagementDbContext>();

    await context.Database.MigrateAsync();
    await DbSeeder.SeedAsync(context);
}

// Middleware pipeline
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

// Enables WebApplicationFactory for future integration tests
public partial class Program { }
