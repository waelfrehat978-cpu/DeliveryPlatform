namespace DeliveryPlatform.Service.Options;

public class OtpOptions
{
    public int ExpiryMinutes { get; set; } = 5;

    public int MaxWrongTries { get; set; } = 5;

    public int SecondsBetweenRequests { get; set; } = 60;

    public int MaxRequestsPerHour { get; set; } = 5;
}/*
  صلاحية الرمز: 5 دقائق.



الحد الأقصى للمحاولات الخاطئة: 5.



الفاصل بين طلبَي OTP: 60 ثانية.



الحد الأقصى للطلبات: 5 خلال ساعة.
  
  */