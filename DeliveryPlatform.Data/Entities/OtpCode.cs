
namespace DeliveryPlatform.Data.Entities;

public class OtpCode : BaseEntity
{
    public string PhoneNumber { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;
    public DateTime? VerifiedAtUtc { get; set; }
    public string? SignupTokenHash { get; set; }
    public bool IsSignupCompleted { get; set; }
    public DateTime ExpiresAtUtc { get; set; }

    public int WrongAttempts { get; set; }

    public bool IsUsed { get; set; }
}
/* 
 * 
PhoneNumber	رقم الهاتف المطلوب التحقق منه
Code	رمز OTP المكوّن من 6 أرقام
ExpiresAtUtc	وقت انتهاء صلاحية الرمز
WrongAttempts	عدد المحاولات الخاطئة
IsUsed	هل استُخدم الرمز بنجاح؟
 VerifiedAtUtc: وقت نجاح التحقق من الرمز.
SignupTokenHash: نخزّن فيها Hash للـ Signup Token.
IsSignupCompleted: يمنع استخدام عملية التحقق نفسها لتسجيل أكثر من حساب.
 */