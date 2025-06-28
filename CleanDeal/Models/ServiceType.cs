using System.ComponentModel.DataAnnotations;

namespace CleanDeal.Models
{
    public class ServiceType
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        [Range(0, 10_000)]
        public decimal BasePrice { get; set; }
    }
}
