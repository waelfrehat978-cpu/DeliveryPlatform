namespace DeliveryPlatform.Service.Exceptions;

public class TooManyRequestsException : AppException
{
    public TooManyRequestsException(string message)
        : base(429, message)
    {
    }
}