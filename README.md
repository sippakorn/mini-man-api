# MiniMan API

A .NET 10 Minimal API solution for managing maintenance notifications, work orders, and access permission requests with **SQL Server temporal tables** for complete history tracking.

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
│       ├── Data/
│       │   └── MiniManDbContext.cs
│       ├── Migrations/
│       │   └── (EF Core migrations)
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
        ├── AccessPermissionRequestEndpointsTests.cs
        └── CustomWebApplicationFactory.cs
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB, Express, or full version)
  - For development, SQL Server LocalDB is recommended (included with Visual Studio)
  - For production, use SQL Server Express or higher

## Database Setup

This application uses **SQL Server with Temporal Tables** for all entities, providing automatic history tracking of all changes.

### Configure Connection String

Update the connection string in `src/MiniMan.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MiniManDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

For other SQL Server instances:
```json
"DefaultConnection": "Server=YOUR_SERVER;Database=MiniManDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True"
```

### Apply Migrations

The database will be created automatically when you first run the application. Alternatively, you can manually apply migrations:

```bash
cd src/MiniMan.Api
dotnet ef database update
```

### Temporal Tables

All three entities (MaintenanceNotifications, WorkOrders, AccessPermissionRequests) are configured as temporal tables with:
- **Current tables**: Store the current state of data
- **History tables**: Automatically track all historical changes
- **Period columns**: `PeriodStart` and `PeriodEnd` track the validity period of each row

**Query temporal data:**
```sql
-- Get current data
SELECT * FROM MaintenanceNotifications

-- Get all historical data
SELECT * FROM MaintenanceNotificationsHistory

-- Get data as of specific point in time
SELECT * FROM MaintenanceNotifications
FOR SYSTEM_TIME AS OF '2026-02-15 12:00:00'

-- Get all changes between two dates
SELECT * FROM MaintenanceNotifications
FOR SYSTEM_TIME BETWEEN '2026-02-01' AND '2026-02-28'
```

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
- ✅ **SQL Server with Temporal Tables** for complete history tracking
- ✅ Entity Framework Core 10 with migrations
- ✅ FluentValidation for request validation
- ✅ OpenAPI/Swagger documentation
- ✅ Proper HTTP status codes (200, 201, 204, 400, 404)
- ✅ Comprehensive unit and integration tests
- ✅ In-memory database for testing
- ✅ XML documentation comments
- ✅ Clean architecture with separation of concerns

## Technology Stack

- **Framework**: .NET 10
- **API Style**: Minimal API
- **Database**: SQL Server with Temporal Tables
- **ORM**: Entity Framework Core 10
- **Validation**: FluentValidation 11.11.0
- **Testing**: xUnit with FluentAssertions, In-Memory Database
- **Documentation**: OpenAPI/Swagger

## Development

### Database Migrations

When you make changes to your models, create a new migration:

```bash
cd src/MiniMan.Api
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

To remove the last migration (if not applied):
```bash
dotnet ef migrations remove
```

To see migration history:
```bash
dotnet ef migrations list
```

### Adding a New Endpoint

1. Add your model to `src/MiniMan.Models/`
2. Update `MiniManDbContext` in `src/MiniMan.Api/Data/` to include the new DbSet
3. Configure the entity with temporal table in `OnModelCreating`
4. Create a migration: `dotnet ef migrations add AddYourEntity`
5. Create a validator in `src/MiniMan.Api/Validators/`
6. Create endpoint methods in `src/MiniMan.Api/Endpoints/`
7. Register the validator in `Program.cs`
8. Map the endpoints in `Program.cs`
9. Add tests in `tests/MiniMan.Api.Tests/`

### Running in Development Mode

```bash
cd src/MiniMan.Api
dotnet watch run
```

This will start the API with hot reload enabled and automatically apply pending migrations.

## Testing

The solution includes 30 integration tests covering:

- Successful CRUD operations
- Validation scenarios
- Error handling (404, 400)
- Edge cases

**Note**: Tests use an in-memory database provider, so temporal table features are not tested. The in-memory provider is automatically configured when running tests.

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

