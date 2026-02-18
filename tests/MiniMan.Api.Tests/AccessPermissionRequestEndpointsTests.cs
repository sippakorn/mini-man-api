using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MiniMan.Models;
using MiniMan.Models.Enums;

namespace MiniMan.Api.Tests;

/// <summary>
/// Integration tests for AccessPermissionRequest endpoints
/// </summary>
[Collection("Sequential")]
public class AccessPermissionRequestEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AccessPermissionRequestEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithList()
    {
        // Act
        var response = await _client.GetAsync("/api/access-permission-requests");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var requests = await response.Content.ReadFromJsonAsync<List<AccessPermissionRequest>>();
        requests.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenDataIsValid()
    {
        // Arrange
        var request = new AccessPermissionRequest
        {
            RequestedBy = "john.doe@example.com",
            ResourceName = "Database Server",
            PermissionType = PermissionType.Read,
            Status = PermissionStatus.Pending,
            Reason = "Need access for reporting"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/access-permission-requests", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdRequest = await response.Content.ReadFromJsonAsync<AccessPermissionRequest>();
        createdRequest.Should().NotBeNull();
        createdRequest!.Id.Should().NotBe(Guid.Empty);
        createdRequest.RequestedBy.Should().Be("john.doe@example.com");
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenRequestedByIsEmpty()
    {
        // Arrange
        var request = new AccessPermissionRequest
        {
            RequestedBy = "",
            ResourceName = "Database Server",
            PermissionType = PermissionType.Write,
            Status = PermissionStatus.Pending,
            Reason = "Need write access"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/access-permission-requests", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenRequestExists()
    {
        // Arrange
        var request = new AccessPermissionRequest
        {
            RequestedBy = "jane.smith@example.com",
            ResourceName = "File Server",
            PermissionType = PermissionType.Admin,
            Status = PermissionStatus.Pending,
            Reason = "System administration duties"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/access-permission-requests", request);
        var createdRequest = await createResponse.Content.ReadFromJsonAsync<AccessPermissionRequest>();

        // Act
        var response = await _client.GetAsync($"/api/access-permission-requests/{createdRequest!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AccessPermissionRequest>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(createdRequest.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenRequestDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/access-permission-requests/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenDataIsValid()
    {
        // Arrange
        var request = new AccessPermissionRequest
        {
            RequestedBy = "bob.wilson@example.com",
            ResourceName = "Application Server",
            PermissionType = PermissionType.Read,
            Status = PermissionStatus.Pending,
            Reason = "Initial request"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/access-permission-requests", request);
        var createdRequest = await createResponse.Content.ReadFromJsonAsync<AccessPermissionRequest>();

        createdRequest!.Status = PermissionStatus.Approved;
        createdRequest.ApprovedDate = DateTime.UtcNow;

        // Act
        var response = await _client.PutAsJsonAsync($"/api/access-permission-requests/{createdRequest.Id}", createdRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedRequest = await response.Content.ReadFromJsonAsync<AccessPermissionRequest>();
        updatedRequest!.Status.Should().Be(PermissionStatus.Approved);
        updatedRequest.ApprovedDate.Should().NotBeNull();
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenRequestDoesNotExist()
    {
        // Arrange
        var request = new AccessPermissionRequest
        {
            Id = Guid.NewGuid(),
            RequestedBy = "nobody@example.com",
            ResourceName = "Unknown Server",
            PermissionType = PermissionType.Read,
            Status = PermissionStatus.Pending,
            Reason = "Test reason"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/access-permission-requests/{request.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenDataIsInvalid()
    {
        // Arrange
        var request = new AccessPermissionRequest
        {
            RequestedBy = "alice.brown@example.com",
            ResourceName = "Web Server",
            PermissionType = PermissionType.Write,
            Status = PermissionStatus.Pending,
            Reason = "Web development tasks"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/access-permission-requests", request);
        var createdRequest = await createResponse.Content.ReadFromJsonAsync<AccessPermissionRequest>();

        createdRequest!.RequestedBy = ""; // Invalid

        // Act
        var response = await _client.PutAsJsonAsync($"/api/access-permission-requests/{createdRequest.Id}", createdRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenRequestExists()
    {
        // Arrange
        var request = new AccessPermissionRequest
        {
            RequestedBy = "charlie.davis@example.com",
            ResourceName = "Test Server",
            PermissionType = PermissionType.Read,
            Status = PermissionStatus.Rejected,
            Reason = "Temporary testing"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/access-permission-requests", request);
        var createdRequest = await createResponse.Content.ReadFromJsonAsync<AccessPermissionRequest>();

        // Act
        var response = await _client.DeleteAsync($"/api/access-permission-requests/{createdRequest!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenRequestDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/access-permission-requests/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
