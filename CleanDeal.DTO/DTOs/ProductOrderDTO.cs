using CleanDeal.Model.Models;

namespace CleanDeal.DTO.DTOs
{
    public class ProductOrderDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Address { get; set; } = string.Empty;
        public DeliveryMethod DeliveryMethod { get; set; }
        public decimal? PaymentAmount { get; set; }
        public IEnumerable<ProductOrderItemDTO>? Items { get; set; }
        public int LoyaltyPoints { get; set; }
    }
}