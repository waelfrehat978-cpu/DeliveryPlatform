namespace DeliveryPlatform.Data.Seed;

public interface IDataSeeder
{
    Task ReseedAsync(
        decimal deliveryFee,
        CancellationToken cancellationToken = default);
}