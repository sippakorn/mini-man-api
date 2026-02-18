# MiniMan API

A .NET 10 Minimal API solution for managing maintenance notifications, work orders, and access permission requests.

## Project Structure

```
MiniMan.sln
├── src/
│   ├── MiniMan.Models/           # Data models and enums
│   │   ├── MaintenanceNotification.cs
│   │   ├── WorkOrder.cs
│   │   ├── AccessPermissionRequest.cs
│   │   └── Enums/
│   │       ├── MaintenanceStatus.cs
│   │       ├── Priority.cs
│   │       ├── WorkOrderStatus.cs
│   │       ├── PermissionType.cs
│   │       └── PermissionStatus.cs
│   └── MiniMan.Api/              # Minimal API project
│       ├── Program.cs
│       ├── Validators/
│       │   ├── MaintenanceNotificationValidator.cs
│       │   ├── WorkOrderValidator.cs
│       │   └── AccessPermissionRequestValidator.cs
│       └── Endpoints/
│           ├── MaintenanceNotificationEndpoints.cs
│           ├── WorkOrderEndpoints.cs
│           └── AccessPermissionRequestEndpoints.cs
└── tests/
    └── MiniMan.Api.Tests/        # xUnit test project
        ├── MaintenanceNotificationEndpointsTests.cs
        ├── WorkOrderEndpointsTests.cs
        └── AccessPermissionRequestEndpointsTests.cs
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/sippakorn/mini-man-api.git
cd mini-man-api
```

### Build the Solution

```bash
dotnet build
```

### Run the API

```bash
cd src/MiniMan.Api
dotnet run
```

The API will start at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

### Run Tests

```bash
dotnet test
```

## API Endpoints

### Maintenance Notifications

- `GET /api/maintenance-notifications` - Get all maintenance notifications
- `GET /api/maintenance-notifications/{id}` - Get a specific maintenance notification
- `POST /api/maintenance-notifications` - Create a new maintenance notification
- `PUT /api/maintenance-notifications/{id}` - Update a maintenance notification
- `DELETE /api/maintenance-notifications/{id}` - Delete a maintenance notification

### Work Orders

- `GET /api/work-orders` - Get all work orders
- `GET /api/work-orders/{id}` - Get a specific work order
- `POST /api/work-orders` - Create a new work order
- `PUT /api/work-orders/{id}` - Update a work order
- `DELETE /api/work-orders/{id}` - Delete a work order

### Access Permission Requests

- `GET /api/access-permission-requests` - Get all access permission requests
- `GET /api/access-permission-requests/{id}` - Get a specific access permission request
- `POST /api/access-permission-requests` - Create a new access permission request
- `PUT /api/access-permission-requests/{id}` - Update an access permission request
- `DELETE /api/access-permission-requests/{id}` - Delete an access permission request

## API Documentation

When running in development mode, OpenAPI documentation is available at:
- `/openapi/v1.json` - OpenAPI specification

You can view the API documentation using tools like [Swagger UI](https://swagger.io/tools/swagger-ui/) or [Postman](https://www.postman.com/).

## Data Models

### MaintenanceNotification

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "Server Maintenance",
  "description": "Scheduled server maintenance",
  "createdDate": "2026-02-18T14:00:00Z",
  "scheduledDate": "2026-02-20T14:00:00Z",
  "status": "Pending",
  "priority": "High"
}
```

### WorkOrder

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "orderNumber": "WO-001",
  "description": "Fix HVAC system",
  "createdDate": "2026-02-18T14:00:00Z",
  "dueDate": "2026-02-25T14:00:00Z",
  "assignedTo": "John Doe",
  "status": "Open",
  "category": "HVAC"
}
```

### AccessPermissionRequest

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "requestedBy": "jane.doe@example.com",
  "resourceName": "Database Server",
  "permissionType": "Read",
  "requestDate": "2026-02-18T14:00:00Z",
  "approvedDate": null,
  "status": "Pending",
  "reason": "Need access for reporting"
}
```

## Validation Rules

The API uses FluentValidation to enforce the following rules:

### MaintenanceNotification
- Title: Required, max 200 characters
- Description: Required, max 2000 characters
- ScheduledDate: Must be in the future (when status is Pending)
- Status: Must be a valid enum value (Pending, InProgress, Completed, Cancelled)
- Priority: Must be a valid enum value (Low, Medium, High, Critical)

### WorkOrder
- OrderNumber: Required, max 50 characters
- Description: Required, max 2000 characters
- DueDate: Must be after CreatedDate
- AssignedTo: Required, max 100 characters
- Status: Must be a valid enum value (Open, InProgress, Completed, Closed)
- Category: Required, max 100 characters

### AccessPermissionRequest
- RequestedBy: Required, max 100 characters
- ResourceName: Required, max 200 characters
- PermissionType: Must be a valid enum value (Read, Write, Admin)
- Status: Must be a valid enum value (Pending, Approved, Rejected)
- Reason: Required, max 1000 characters
- ApprovedDate: Must be after RequestDate (when provided)

## Features

- ✅ .NET 10 Minimal API
- ✅ Three entity types with full CRUD operations
- ✅ FluentValidation for request validation
- ✅ In-memory data storage
- ✅ OpenAPI/Swagger documentation
- ✅ Proper HTTP status codes (200, 201, 204, 400, 404)
- ✅ Comprehensive unit and integration tests
- ✅ XML documentation comments
- ✅ Clean architecture with separation of concerns

## Technology Stack

- **Framework**: .NET 10
- **API Style**: Minimal API
- **Validation**: FluentValidation 11.11.0
- **Testing**: xUnit with FluentAssertions
- **Documentation**: OpenAPI/Swagger

## Development

### Adding a New Endpoint

1. Add your model to `src/MiniMan.Models/`
2. Create a validator in `src/MiniMan.Api/Validators/`
3. Create endpoint methods in `src/MiniMan.Api/Endpoints/`
4. Register the validator in `Program.cs`
5. Map the endpoints in `Program.cs`
6. Add tests in `tests/MiniMan.Api.Tests/`

### Running in Development Mode

```bash
cd src/MiniMan.Api
dotnet watch run
```

This will start the API with hot reload enabled.

## Testing

The solution includes 30 integration tests covering:

- Successful CRUD operations
- Validation scenarios
- Error handling (404, 400)
- Edge cases

To run specific tests:

```bash
dotnet test --filter "FullyQualifiedName~MaintenanceNotification"
```

To run tests with detailed output:

```bash
dotnet test --verbosity normal
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License.

## Contact

Project Link: [https://github.com/sippakorn/mini-man-api](https://github.com/sippakorn/mini-man-api)

