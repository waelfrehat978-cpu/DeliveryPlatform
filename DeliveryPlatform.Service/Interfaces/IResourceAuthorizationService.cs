namespace DeliveryPlatform.Service.Interfaces;

public interface IResourceAuthorizationService
{
    Task EnsureStoreAccessAsync(int storeId);

    Task EnsureProductAccessAsync(int productId);

    Task EnsureCustomerAccessAsync(int customerId);

    Task EnsureOrderAccessAsync(int orderId);

}
/*
 التحقق من صلاحية المستخدم للوصول إلى موارد معينة
 1. EnsureStoreAccessAsync: يتحقق أن الـ Partner يصل إلى متجره فقط.
2. EnsureProductAccessAsync: يتحقق أن المنتج تابع لمتجر الـ Partner.
3. EnsureCustomerAccessAsync: يتحقق أن العميل يصل إلى بياناته فقط.
هذه الدوال لا ترجع بيانات؛ وظيفتها التحقق من الصلاحية، وإذا لم تتحقق ترمي Exception
 
 */