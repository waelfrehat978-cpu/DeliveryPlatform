namespace DeliveryPlatform.Service.Exceptions;

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message)
        : base(401, message)
    {
    }
}
//401 التوكن غير فعاله 