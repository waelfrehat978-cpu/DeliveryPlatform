namespace DeliveryPlatform.Service.Interfaces;

public interface IClockService
{
    DateTime UtcNow { get; }
}