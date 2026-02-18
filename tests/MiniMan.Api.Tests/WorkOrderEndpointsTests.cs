using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using MiniMan.Models;
using MiniMan.Models.Enums;

namespace MiniMan.Api.Tests;

/// <summary>
/// Integration tests for WorkOrder endpoints
/// </summary>
public class WorkOrderEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public WorkOrderEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithList()
    {
        // Act
        var response = await _client.GetAsync("/api/work-orders");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var workOrders = await response.Content.ReadFromJsonAsync<List<WorkOrder>>();
        workOrders.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenDataIsValid()
    {
        // Arrange
        var workOrder = new WorkOrder
        {
            OrderNumber = "WO-001",
            Description = "Test Work Order",
            DueDate = DateTime.UtcNow.AddDays(7),
            AssignedTo = "John Doe",
            Status = WorkOrderStatus.Open,
            Category = "Maintenance"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/work-orders", workOrder);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdWorkOrder = await response.Content.ReadFromJsonAsync<WorkOrder>();
        createdWorkOrder.Should().NotBeNull();
        createdWorkOrder!.Id.Should().NotBe(Guid.Empty);
        createdWorkOrder.OrderNumber.Should().Be("WO-001");
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenOrderNumberIsEmpty()
    {
        // Arrange
        var workOrder = new WorkOrder
        {
            OrderNumber = "",
            Description = "Test Work Order",
            DueDate = DateTime.UtcNow.AddDays(7),
            AssignedTo = "John Doe",
            Status = WorkOrderStatus.Open,
            Category = "Maintenance"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/work-orders", workOrder);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenWorkOrderExists()
    {
        // Arrange
        var workOrder = new WorkOrder
        {
            OrderNumber = "WO-002",
            Description = "Test Work Order",
            DueDate = DateTime.UtcNow.AddDays(5),
            AssignedTo = "Jane Smith",
            Status = WorkOrderStatus.InProgress,
            Category = "Repair"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/work-orders", workOrder);
        var createdWorkOrder = await createResponse.Content.ReadFromJsonAsync<WorkOrder>();

        // Act
        var response = await _client.GetAsync($"/api/work-orders/{createdWorkOrder!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<WorkOrder>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(createdWorkOrder.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenWorkOrderDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/work-orders/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenDataIsValid()
    {
        // Arrange
        var workOrder = new WorkOrder
        {
            OrderNumber = "WO-003",
            Description = "Original Description",
            DueDate = DateTime.UtcNow.AddDays(10),
            AssignedTo = "Bob Wilson",
            Status = WorkOrderStatus.Open,
            Category = "Installation"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/work-orders", workOrder);
        var createdWorkOrder = await createResponse.Content.ReadFromJsonAsync<WorkOrder>();

        createdWorkOrder!.Description = "Updated Description";
        createdWorkOrder.Status = WorkOrderStatus.Completed;

        // Act
        var response = await _client.PutAsJsonAsync($"/api/work-orders/{createdWorkOrder.Id}", createdWorkOrder);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedWorkOrder = await response.Content.ReadFromJsonAsync<WorkOrder>();
        updatedWorkOrder!.Description.Should().Be("Updated Description");
        updatedWorkOrder.Status.Should().Be(WorkOrderStatus.Completed);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenWorkOrderDoesNotExist()
    {
        // Arrange
        var workOrder = new WorkOrder
        {
            Id = Guid.NewGuid(),
            OrderNumber = "WO-999",
            Description = "Test Work Order",
            DueDate = DateTime.UtcNow.AddDays(5),
            AssignedTo = "Nobody",
            Status = WorkOrderStatus.Open,
            Category = "Test"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/work-orders/{workOrder.Id}", workOrder);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenDataIsInvalid()
    {
        // Arrange
        var workOrder = new WorkOrder
        {
            OrderNumber = "WO-004",
            Description = "Test Description",
            DueDate = DateTime.UtcNow.AddDays(3),
            AssignedTo = "Alice Brown",
            Status = WorkOrderStatus.Open,
            Category = "Testing"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/work-orders", workOrder);
        var createdWorkOrder = await createResponse.Content.ReadFromJsonAsync<WorkOrder>();

        createdWorkOrder!.OrderNumber = ""; // Invalid

        // Act
        var response = await _client.PutAsJsonAsync($"/api/work-orders/{createdWorkOrder.Id}", createdWorkOrder);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenWorkOrderExists()
    {
        // Arrange
        var workOrder = new WorkOrder
        {
            OrderNumber = "WO-005",
            Description = "Test Work Order",
            DueDate = DateTime.UtcNow.AddDays(2),
            AssignedTo = "Charlie Davis",
            Status = WorkOrderStatus.Open,
            Category = "Cleanup"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/work-orders", workOrder);
        var createdWorkOrder = await createResponse.Content.ReadFromJsonAsync<WorkOrder>();

        // Act
        var response = await _client.DeleteAsync($"/api/work-orders/{createdWorkOrder!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenWorkOrderDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/work-orders/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
