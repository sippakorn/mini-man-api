using FluentValidation;
using MiniMan.Api.Endpoints;
using MiniMan.Api.Validators;
using MiniMan.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();

// Register FluentValidation validators
builder.Services.AddScoped<IValidator<MaintenanceNotification>, MaintenanceNotificationValidator>();
builder.Services.AddScoped<IValidator<WorkOrder>, WorkOrderValidator>();
builder.Services.AddScoped<IValidator<AccessPermissionRequest>, AccessPermissionRequestValidator>();

var app = builder.Build();

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
