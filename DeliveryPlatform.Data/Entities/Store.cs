using DeliveryPlatform.Data.Auditing;

namespace DeliveryPlatform.Data.Entities
{

    [AuditedIn(typeof(StoreAuditTrail))]
    public class Store : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }
}