
using DeliveryPlatform.Data.Auditing;

namespace DeliveryPlatform.Data.Entities
{
    [AuditedIn(typeof(CustomerAuditTrail))]
    public class Customer : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}
