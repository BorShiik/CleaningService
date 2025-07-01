using System.ComponentModel.DataAnnotations;

namespace CleanDeal.Model.Models
{
    public enum DeliveryMethod
    {
        Courier,
        PickupPoint
    }

    public class ProductOrder
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        [Required, StringLength(200)]
        public string Address { get; set; } = string.Empty;
        [Required]
        public DeliveryMethod DeliveryMethod { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public ICollection<ProductOrderItem> Items { get; set; } = new List<ProductOrderItem>();
        public Payment? Payment { get; set; }
    }
}