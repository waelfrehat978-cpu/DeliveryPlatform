using DeliveryPlatform.Data.Entities;

namespace DeliveryPlatform.Service.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(User user);
    }
}/*
  
  ستقبل المستخدم بعد نجاح تسجيل الدخول.



يرجع Access Token بصيغة JWT.



سنستخدمه لاحقاً في تسجيل دخول Customer وPartner وAdmin.
  */