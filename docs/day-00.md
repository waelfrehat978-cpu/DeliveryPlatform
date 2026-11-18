# Day 0 – Project Foundation

## 1. Project Overview

The goal of this project is to build a multi-store delivery platform backend using ASP.NET Core and Entity Framework Core.

The platform allows customers to browse stores, view products, and place orders. Stores can manage their products and process incoming orders.

## 2. Solution Architecture

The solution is divided into three projects:

- **DeliveryPlatform.Api** – Handles HTTP requests, controllers, middleware, and API configuration.
- **DeliveryPlatform.Service** – Contains business logic, service interfaces, and Data Transfer Objects (DTOs).
- **DeliveryPlatform.Data** – Contains database entities, Entity Framework Core configurations, DbContext, migrations, and data seeding.

Project dependencies:

- Api → Service
- Service → Data

This structure separates responsibilities and keeps business logic independent of the API layer.

## 3. Dependency Injection

Dependency Injection is used to provide services and their dependencies through constructors.

Service registrations are configured in `Program.cs`.

This avoids manually creating dependencies and makes the application easier to maintain and test.

## 4. Database Configuration

The project uses SQL Server with Entity Framework Core.

The connection string is stored in configuration rather than being hardcoded in application classes.

`DeliveryPlatformDbContext` manages the database entities and their relationships.

## 5. Base Entity

A shared `BaseEntity` contains common properties used by database entities:

- `Id`
- `CreatedAtUtc`
- `UpdatedAtUtc`
- `IsDeleted`
- `DeletedAtUtc`

This avoids repeating common fields across entities.

## 6. Entity Relationships and Delete Behavior

Entity relationships are configured using Entity Framework Core Fluent API configurations.

`DeleteBehavior.Restrict` is used to prevent deleting records that are still referenced by related records.

## 7. Soft Delete and Audit Timestamps

A `SaveChangesInterceptor` is used to:

- Set creation timestamps automatically.
- Update modification timestamps.
- Convert physical delete operations into soft deletes.
- Maintain deletion timestamps.

A global query filter excludes soft-deleted records from normal queries.

## 8. Error Handling

Centralized exception handling middleware is used to convert application exceptions into appropriate HTTP responses.

This keeps error handling consistent across API endpoints.

## 9. Project Structure

The solution follows a layered structure:

```text
DeliveryPlatform
│
├── DeliveryPlatform.Api
│   ├── Controllers
│   ├── Middleware
│   └── Program.cs
│
├── DeliveryPlatform.Service
│   ├── DTOs
│   ├── Interfaces
│   └── Services
│
└── DeliveryPlatform.Data
    ├── Context
    ├── Configurations
    ├── Entities
    ├── Enums
    ├── Interceptors
    ├── Migrations
    └── Seed
```

## 10. What I Learned

- How to organize an ASP.NET Core solution into separate layers.
- How Dependency Injection works.
- How Entity Framework Core manages database relationships.
- How to configure SQL Server and database migrations.
- How to implement centralized exception handling.
- How soft deletion and audit timestamps work.

## 11. Challenges

The main challenge was understanding how the different layers communicate while keeping each layer responsible for its own tasks.

I addressed this by implementing the project step by step and testing each part before moving forward.