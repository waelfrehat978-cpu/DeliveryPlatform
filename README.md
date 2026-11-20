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
5. Open `appsettings.json` and fill in your connection string.
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

9. Open `http://localhost:5131/swagger`.
