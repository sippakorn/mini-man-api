using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MiniMan.Models;
using MiniMan.Models.Enums;

namespace MiniMan.Api.Tests;

/// <summary>
/// Integration tests for MaintenanceNotification endpoints
/// </summary>
[Collection("Sequential")]
public class MaintenanceNotificationEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public MaintenanceNotificationEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithList()
    {
        // Act
        var response = await _client.GetAsync("/api/maintenance-notifications");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var notifications = await response.Content.ReadFromJsonAsync<List<MaintenanceNotification>>();
        notifications.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenDataIsValid()
    {
        // Arrange
        var notification = new MaintenanceNotification
        {
            Title = "Test Notification",
            Description = "Test Description",
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            Status = MaintenanceStatus.Pending,
            Priority = Priority.High
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/maintenance-notifications", notification);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdNotification = await response.Content.ReadFromJsonAsync<MaintenanceNotification>();
        createdNotification.Should().NotBeNull();
        createdNotification!.Id.Should().NotBe(Guid.Empty);
        createdNotification.Title.Should().Be("Test Notification");
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenTitleIsEmpty()
    {
        // Arrange
        var notification = new MaintenanceNotification
        {
            Title = "",
            Description = "Test Description",
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            Status = MaintenanceStatus.Pending,
            Priority = Priority.High
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/maintenance-notifications", notification);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenNotificationExists()
    {
        // Arrange
        var notification = new MaintenanceNotification
        {
            Title = "Test Notification",
            Description = "Test Description",
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            Status = MaintenanceStatus.Pending,
            Priority = Priority.Medium
        };
        var createResponse = await _client.PostAsJsonAsync("/api/maintenance-notifications", notification);
        var createdNotification = await createResponse.Content.ReadFromJsonAsync<MaintenanceNotification>();

        // Act
        var response = await _client.GetAsync($"/api/maintenance-notifications/{createdNotification!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<MaintenanceNotification>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(createdNotification.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenNotificationDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/maintenance-notifications/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenDataIsValid()
    {
        // Arrange
        var notification = new MaintenanceNotification
        {
            Title = "Original Title",
            Description = "Original Description",
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            Status = MaintenanceStatus.Pending,
            Priority = Priority.Low
        };
        var createResponse = await _client.PostAsJsonAsync("/api/maintenance-notifications", notification);
        var createdNotification = await createResponse.Content.ReadFromJsonAsync<MaintenanceNotification>();

        createdNotification!.Title = "Updated Title";

        // Act
        var response = await _client.PutAsJsonAsync($"/api/maintenance-notifications/{createdNotification.Id}", createdNotification);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedNotification = await response.Content.ReadFromJsonAsync<MaintenanceNotification>();
        updatedNotification!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenNotificationDoesNotExist()
    {
        // Arrange
        var notification = new MaintenanceNotification
        {
            Id = Guid.NewGuid(),
            Title = "Test Notification",
            Description = "Test Description",
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            Status = MaintenanceStatus.Pending,
            Priority = Priority.High
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/maintenance-notifications/{notification.Id}", notification);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenDataIsInvalid()
    {
        // Arrange
        var notification = new MaintenanceNotification
        {
            Title = "Original Title",
            Description = "Original Description",
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            Status = MaintenanceStatus.Pending,
            Priority = Priority.Critical
        };
        var createResponse = await _client.PostAsJsonAsync("/api/maintenance-notifications", notification);
        var createdNotification = await createResponse.Content.ReadFromJsonAsync<MaintenanceNotification>();

        createdNotification!.Title = ""; // Invalid

        // Act
        var response = await _client.PutAsJsonAsync($"/api/maintenance-notifications/{createdNotification.Id}", createdNotification);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenNotificationExists()
    {
        // Arrange
        var notification = new MaintenanceNotification
        {
            Title = "Test Notification",
            Description = "Test Description",
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            Status = MaintenanceStatus.Pending,
            Priority = Priority.Medium
        };
        var createResponse = await _client.PostAsJsonAsync("/api/maintenance-notifications", notification);
        var createdNotification = await createResponse.Content.ReadFromJsonAsync<MaintenanceNotification>();

        // Act
        var response = await _client.DeleteAsync($"/api/maintenance-notifications/{createdNotification!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenNotificationDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/maintenance-notifications/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
