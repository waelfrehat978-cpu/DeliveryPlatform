using DeliveryPlatform.Data.Auditing;

namespace DeliveryPlatform.Data.Entities
{

    [AuditedIn(typeof(CustomerAuditTrail))]
    public class Address : BaseEntity
    {
        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;

        public string Label { get; set; } = string.Empty;

        public string Street { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public bool IsDefault { get; set; }
    }
}