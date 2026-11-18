using DeliveryPlatform.Service.Interfaces;

namespace DeliveryPlatform.Service.Services;

public class ClockService : IClockService
{
    public DateTime UtcNow => DateTime.UtcNow;
}