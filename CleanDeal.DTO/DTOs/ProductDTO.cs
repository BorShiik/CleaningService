using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using CleanDeal.Model.Models;

namespace CleanDeal.DTO.DTOs
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
        public IFormFile? ImageFile { get; set; }
        public byte[]? Image { get; set; }
        public string? ImageMimeType { get; set; }
    }
}
