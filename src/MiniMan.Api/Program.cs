using FluentValidation;
using MiniMan.Api.Data;
using MiniMan.Api.Endpoints;
using MiniMan.Api.Validators;
using MiniMan.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();

// Register DbContext - use in-memory for testing, SQL Server otherwise
if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<MiniManDbContext>(options =>
        options.UseInMemoryDatabase("TestDatabase"));
}
else
{
    builder.Services.AddDbContext<MiniManDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Register FluentValidation validators
builder.Services.AddScoped<IValidator<MaintenanceNotification>, MaintenanceNotificationValidator>();
builder.Services.AddScoped<IValidator<WorkOrder>, WorkOrderValidator>();
builder.Services.AddScoped<IValidator<AccessPermissionRequest>, AccessPermissionRequestValidator>();

var app = builder.Build();

// Apply migrations and create database (skip for testing environment)
if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<MiniManDbContext>();
        dbContext.Database.Migrate();
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map endpoints for all entities
app.MapMaintenanceNotificationEndpoints();
app.MapWorkOrderEndpoints();
app.MapAccessPermissionRequestEndpoints();

app.Run();

// Make the implicit Program class public for testing
public partial class Program { }
