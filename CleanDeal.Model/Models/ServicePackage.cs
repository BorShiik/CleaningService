using System.ComponentModel.DataAnnotations;

namespace CleanDeal.Model.Models
{
    public class ServicePackage
    {
        public int Id { get; set; }
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Range(0, 10000)]
        public decimal Price { get; set; }
        public ICollection<ServicePackageItem> Items { get; set; } = new List<ServicePackageItem>();
    }
}