using DeliveryPlatform.Data.Enums;
namespace DeliveryPlatform.Data.Entities
{
    public class Order : BaseEntity
    {
        public string OrderCode { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;

        public int StoreId { get; set; }

        public Store Store { get; set; } = null!;

        public int AddressId { get; set; }

        public Address Address { get; set; } = null!;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public decimal Subtotal { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal Total { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public ICollection<OrderLine> OrderLines { get; set; }
    = new List<OrderLine>();
        public ICollection<OrderStatusHistory> StatusHistory { get; set; }
    = new List<OrderStatusHistory>();
    }
}