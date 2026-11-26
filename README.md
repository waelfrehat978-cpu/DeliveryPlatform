# DeliveryPlatform

A multi-store delivery platform backend built with ASP.NET Core 10 and Entity Framework Core 10.

The project provides customer, partner, and admin authentication, store and product management, order processing, role-based authorization, refresh-token rotation, OTP verification, audit trails, and application logging.

## Project Structure

The solution contains three main projects:

- `DeliveryPlatform.Api`: ASP.NET Core Web API, controllers, middleware, authentication configuration, and application startup.
- `DeliveryPlatform.Service`: Business logic, services, DTOs, authorization, and application interfaces.
- `DeliveryPlatform.Data`: EF Core DbContext, entities, database configuration, migrations, and data seeding.

## Requirements

- .NET 10 SDK
- SQL Server
- Entity Framework Core tools

## Run It

1. Install SQL Server and the .NET 10 SDK.

2. Install the EF Core tool once:

   ```bash
   dotnet tool install --global dotnet-ef
   ```

3. Clone this repository:

   ```bash
   git clone https://github.com/waelfrehat978-cpu/DeliveryPlatform.git
   cd DeliveryPlatform
   ```

4. Copy the example configuration:

   ```text
   DeliveryPlatform.Api/appsettings.Example.json
   ```

   to:

   ```text
   DeliveryPlatform.Api/appsettings.json
   ```

5. Open `DeliveryPlatform.Api/appsettings.json` and configure your connection string, JWT key, and seed credentials.

6. Create or update the database:

   ```bash
   dotnet ef database update --project DeliveryPlatform.Data --startup-project DeliveryPlatform.Api
   ```

7. Fill the database with test data:

   ```bash
   dotnet run --project DeliveryPlatform.Api -- --reseed
   ```

   **Warning:** The `--reseed` option is intended for development and testing. It resets seeded data and may delete existing database records. Do not run it against a database containing data you need to keep.

8. Run the API:

   ```bash
   dotnet run --project DeliveryPlatform.Api
   ```

9. Open the API documentation:

   `http://localhost:5131/swagger`

   The API base URL is:

   `https://localhost:7195`

   These are the default local development URLs. Actual URLs may depend on your launch settings.

## Configuration

Before running the application, configure the following sections in `DeliveryPlatform.Api/appsettings.json`.

### ConnectionStrings

- `ConnectionStrings:DefaultConnection`: SQL Server connection string.

### Order

- `Order:DeliveryFee`: Default delivery fee. The example configuration uses `2.50`.

### Jwt

- `Jwt:Key`: Secret signing key. Use a secure key of at least 32 characters.
- `Jwt:Issuer`: Token issuer.
- `Jwt:Audience`: Token audience.
- `Jwt:AccessTokenMinutes`: Access token lifetime in minutes.
- `Jwt:RefreshTokenDays`: Refresh token lifetime in days.

Never commit real JWT keys or other secrets to source control.

### Otp

- `Otp:ExpiryMinutes`: OTP validity period.
- `Otp:MaxWrongTries`: Maximum incorrect verification attempts.
- `Otp:SecondsBetweenRequests`: Minimum time between OTP requests.
- `Otp:MaxRequestsPerHour`: Maximum OTP requests allowed per hour.

### Seed

- `Seed:AdminEmail`: Admin login email.
- `Seed:AdminPassword`: Admin login password.
- `Seed:PartnerPassword`: Shared password for seeded Partner accounts.

Do not commit real passwords or secret values to source control.

## Seeded Login Credentials

The seeder creates the following accounts:

| Role | Login | Password |
|---|---|---|
| Admin | Value of `Seed:AdminEmail` | Value of `Seed:AdminPassword` |
| Partner | `partner{StoreId}@example.com` | Value of `Seed:PartnerPassword` |
| Customer | Phone number in the format `079` + Customer ID padded to 7 digits | OTP verification |

Each seeded Store has one Partner account, and each seeded Customer has one Customer account.

## Authentication

The API supports different authentication flows depending on the user role.

### Customer Authentication

Customers authenticate using their phone number and a one-time password (OTP).

The general flow is:

1. Request an OTP using the customer's phone number.
2. Verify the OTP.
3. Complete signup if the customer is registering for the first time.
4. Use the returned access token to access protected endpoints.

Customer accounts are created only after successful OTP verification and signup completion.

**Development note:** The current `LogSmsSender` implementation writes OTP codes to the API console log for local development and testing. It does not represent a production SMS delivery integration.

### Partner and Admin Authentication

Partners and Admins authenticate using their email address and password through the staff login endpoint.

The API returns access and refresh tokens after successful authentication.

## Token Security

- Access tokens are JWTs.
- Refresh tokens are generated using secure random values.
- Only hashed refresh tokens are stored in the database.
- Refresh tokens are rotated when used.
- Reuse of a revoked refresh token is detected and handled by the authentication service.

## Authorization

The API supports three roles:

- `Customer`
- `Partner`
- `Admin`

Protected resources are checked against the authenticated user's permissions and ownership.

For example, Partner access to store-related resources is validated against the Partner's assigned Store.

## Logging and Audit Trails

The application uses structured application logging to record application events and errors.

Audit trails record selected changes to supported entities, including the action, affected entity, timestamp, and user where available.

Application logs and audit trails serve different purposes:

- **Logging:** Helps diagnose application behavior and errors.
- **Audit trails:** Record selected data changes for traceability.

## Development Notes

- Use local development credentials only for testing.
- Do not commit `appsettings.json` if it contains real secrets.
- Do not expose OTP codes through production logging.
- Do not use the development `LogSmsSender` as a production SMS provider.
- Avoid running `--reseed` against databases containing important data.