# Day 0 — Project Foundation

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

### ClockService Registration Experiment

The Day 0 exercise demonstrated the difference between registering a service and injecting it.

When `ClockService` was injected into `StoresController` without being registered, the application produced this error:

```text
Unable to resolve service for type 'ClockService' while attempting to activate 'StoresController'.
```

The problem was fixed by registering the service:

```csharp
builder.Services.AddScoped<ClockService>();
```

This demonstrated that registering and injecting a dependency are both necessary.

## 4. Service Interfaces

Every service is accessed through an interface rather than depending directly on its implementation.

For example:

```csharp
builder.Services.AddScoped<IOrderService, OrderService>();
```

The controller requests `IOrderService`, and Dependency Injection provides its registered implementation.

This follows Dependency Inversion and makes services easier to replace and test.

## 5. Database Configuration

The project uses SQL Server with Entity Framework Core.

The connection string is stored in configuration rather than being hardcoded in application classes.

`DeliveryPlatformDbContext` manages the database entities and their relationships.

## 6. Base Entity

A shared `BaseEntity` contains common properties used by database entities:

- `Id`
- `CreatedAtUtc`
- `UpdatedAtUtc`
- `IsDeleted`
- `DeletedAtUtc`

This avoids repeating common fields across entities.

## 7. Entity Relationships and Delete Behavior

Entity relationships are configured using Entity Framework Core Fluent API configurations.

`DeleteBehavior.Restrict` is used to prevent deleting records that are still referenced by related records.

## 8. Soft Delete and Audit Timestamps

A `SaveChangesInterceptor` is used to:

- Set creation timestamps automatically.
- Update modification timestamps.
- Convert physical delete operations into soft deletes.
- Maintain deletion timestamps.

A global query filter excludes soft-deleted records from normal queries.

## 9. Error Handling

Centralized exception handling middleware is used to convert application exceptions into appropriate HTTP responses.

This keeps error handling consistent across API endpoints.

## 10. Project Structure

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

## 11. Dependency Injection Lifetimes Experiment

The Day 0 exercise used three service lifetimes:

- **Transient:** A new instance is created every time the service is requested.
- **Scoped:** One instance is shared within the same scope (HTTP request).
- **Singleton:** One instance is shared throughout the application's lifetime.

The exercise used two instances of each stamp class and returned their GUIDs from one endpoint.

### Expected Results

| Lifetime | First instance | Second instance | Across requests |
|---|---|---|---|
| Transient | Different | Different | New instances |
| Scoped | Same | Same | New instance per request |
| Singleton | Same | Same | Same instance |

### Six GUID Values

The original six GUID values were not available when this document was prepared, so they have not been fabricated.

| Instance | GUID |
|---|---|
| t1 | Not recorded |
| t2 | Not recorded |
| s1 | Not recorded |
| s2 | Not recorded |
| g1 | Not recorded |
| g2 | Not recorded |

## 12. Singleton and Scoped Dependency Error

The exercise changed `SingletonStamp` to require `ScopedStamp` in its constructor.

The application produced this error:

```text
Cannot consume scoped service 'ScopedStamp' from singleton 'SingletonStamp'.
```

This happens because a singleton lives for the entire application lifetime, while a scoped service belongs to one scope.

A singleton must not directly depend on a scoped service.

The experiment demonstrated why service lifetimes must be selected carefully.

## 13. Configuration and Secrets

Application settings are stored in configuration files instead of being hardcoded in application classes.

Sensitive values such as connection strings and JWT signing keys should not be committed to Git.

An example configuration file provides placeholders so another developer can configure and run the application.

## 14. What I Learned

- How to organize an ASP.NET Core solution into separate layers.
- How Dependency Injection works.
- The difference between registering and injecting a service.
- The differences between Transient, Scoped, and Singleton lifetimes.
- Why a singleton must not directly depend on a scoped service.
- How Entity Framework Core manages database relationships.
- How to configure SQL Server and database migrations.
- How to implement centralized exception handling.
- How soft deletion and audit timestamps work.

## 15. Challenges

The main challenge was understanding how the different layers communicate while keeping each layer responsible for its own tasks.

I addressed this by implementing the project step by step and testing each part before moving forward.