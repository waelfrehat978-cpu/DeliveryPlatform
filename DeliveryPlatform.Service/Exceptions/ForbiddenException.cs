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
/*
401 المستخدم غير مصدق عليه   	Token غير صالح أو غير موجود
 403 المستخدم لا يمتلك صلاحيه
 */