

namespace DeliveryPlatform.Service.Exceptions
{
  public class ConflictException:AppException
    {

        public ConflictException(string message) : base(409,message) { }
    }
}
