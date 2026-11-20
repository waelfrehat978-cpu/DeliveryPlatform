
using DeliveryPlatform.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace DeliveryPlatform.Service.Services;

public class LogSmsSender : ISmsSender
{
    private readonly ILogger<LogSmsSender> _logger;

    public LogSmsSender(ILogger<LogSmsSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string phoneNumber, string message)
    {
        _logger.LogInformation(
            "SMS sent to {PhoneNumber}. Message: {Message}",
            phoneNumber,
            message);

        return Task.CompletedTask;
    }
}
