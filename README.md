# DeliveryPlatform

A multi-store delivery platform backend built with ASP.NET Core and Entity Framework Core.

## Run it

1. Install SQL Server and the .NET 10 SDK.

2. Install the EF Core tool once:

   ```bash
   dotnet tool install --global dotnet-ef
   ```

3. Clone this repository.

4. Copy `DeliveryPlatform.Api/appsettings.Example.json` to `DeliveryPlatform.Api/appsettings.json`.

5. Open `appsettings.json` and configure your connection string, JWT key, and Seed credentials.

6. Create the database:

   ```bash
   dotnet ef database update --project DeliveryPlatform.Data --startup-project DeliveryPlatform.Api
   ```

7. Fill it with test data:

   ```bash
   dotnet run --project DeliveryPlatform.Api -- --reseed
   ```

8. Run the API:

   ```bash
   dotnet run --project DeliveryPlatform.Api
   ```

9. Open the API documentation at:

   `http://localhost:5131/swagger`

## Seed Configuration

Before running the seeder, configure the following settings in `DeliveryPlatform.Api/appsettings.json`:

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

Customer accounts authenticate using the OTP flow.