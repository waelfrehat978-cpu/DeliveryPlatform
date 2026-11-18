namespace DeliveryPlatform.Service.Interfaces;

public interface ISmsSender
{
    Task SendAsync(string phoneNumber, string message);
}