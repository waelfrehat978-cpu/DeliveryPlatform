using System.ComponentModel.DataAnnotations;

namespace DeliveryPlatform.Service.DTOs.Orders
{
    public class OrderLineResponseDto
    {

       
        public int ProductId { get; set; }

       
        public string ProductName { get; set; } = string.Empty;

        
        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal LineTotal { get; set; }
    }
}