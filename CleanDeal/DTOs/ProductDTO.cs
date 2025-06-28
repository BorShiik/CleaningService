using CleanDeal.Models;
using System.ComponentModel.DataAnnotations;

namespace CleanDeal.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }  = string.Empty;
        public decimal Price { get; set; }
        [Range(0, 50)]
        public int StockQuantity { get; set; }
        public ProductCategory Category { get; set; }
        public string? ImageUrl { get; set; }
    }
}
