using System.Text;
using System.Security.Cryptography;
namespace DeliveryPlatform.Service.Security;

public static class SecureToken
{

    public static string Create()
    {
        var randomBytes=RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(randomBytes);
    }
    public static string Hash(string token)
    {
        var tokenBytes=Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);
        return Convert.ToBase64String(hashBytes);
    }


}/*
  
Create()	إنشاء Token عشوائي
Hash(token)	تحويل Token إلى SHA256 Hash
  
  */