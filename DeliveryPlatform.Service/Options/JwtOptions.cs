namespace DeliveryPlatform.Service.Options
{
    public class JwtOptions
    {
        public string Key { get; set; } = string.Empty;

        public string Issuer { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        public int AccessTokenMinutes { get; set; } = 15;

        public int RefreshTokenDays { get; set; } = 7;
    }
}
/*
 
 SectionName: اسم القسم الذي سنقرأ منه الإعدادات في appsettings.json.



Key: المفتاح السري المستخدم لتوقيع JWT.



Issuer: الجهة التي أصدرت التوكن.



Audience: الجهة المستهدفة بالتوكن.



AccessTokenMinutes: مدة صلاحية Access Token.



RefreshTokenDays: مدة صلاحية Refresh Token
 */