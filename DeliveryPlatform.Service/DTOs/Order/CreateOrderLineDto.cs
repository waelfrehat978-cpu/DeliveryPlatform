using System.ComponentModel.DataAnnotations;

namespace DeliveryPlatform.Service.DTOs.Orders
{
    public class CreateOrderLineDto
    {
        [Range(1,int.MaxValue)]
        public int ProductId { get; set; }
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}