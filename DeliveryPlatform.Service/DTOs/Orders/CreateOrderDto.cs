using DeliveryPlatform.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace DeliveryPlatform.Service.DTOs.Orders
{
    public class CreateOrderDto
    {
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }
        [Range(1, int.MaxValue)]
        public int StoreId { get; set; }
        [Range(1, int.MaxValue)]
        public int AddressId { get; set; }
        [EnumDataType(typeof(PaymentMethod))]
        public PaymentMethod PaymentMethod { get; set; }
        [Required,MinLength (1)]
        public List<CreateOrderLineDto> Items { get; set; } = new();
    }
}