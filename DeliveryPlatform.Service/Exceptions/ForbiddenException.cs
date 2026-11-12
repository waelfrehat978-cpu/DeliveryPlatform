namespace DeliveryPlatform.Service.Exceptions
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message)
            : base(403, message)
        {
        }
    }
}