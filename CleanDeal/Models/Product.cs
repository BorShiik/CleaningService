using System.ComponentModel.DataAnnotations;

namespace CleanDeal.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        [Range(0, 50)]
        public int StockQuantity { get; set; }
    }
}
