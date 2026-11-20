
using DeliveryPlatform.Data.Enums;

namespace DeliveryPlatform.Data.Interfaces;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    int? UserId { get; }

    UserRole? Role { get; }

    int? StoreId { get; }

    int? CustomerId { get; }
}
/*
 
 IsAuthenticated: هل المستخدم مسجل دخوله؟



UserId: رقم المستخدم، واستخدمنا int? لأن القيمة قد تكون غير موجودة.



Role: دور المستخدم، مثل Customer أو Partner أو Admin.



StoreId: رقم المتجر، ويكون له قيمة عندما يكون المستخدم Partner.



CustomerId: رقم العميل، ويكون له قيمة عندما يكون المستخدم Customer.
 */