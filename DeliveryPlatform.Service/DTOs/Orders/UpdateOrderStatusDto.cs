using DeliveryPlatform.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace DeliveryPlatform.Service.DTOs.Orders
{
    public class UpdateOrderStatusDto
    {
        [EnumDataType(typeof(OrderStatus))]
        public OrderStatus Status { get; set; }
        [StringLength(500)]
        public string? Note { get; set; }
    }
}