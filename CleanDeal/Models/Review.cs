using Microsoft.Identity.Client;

namespace CleanDeal.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating {  get; set; }
        public string? Comment {  get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int CleaningOrderId { get; set; }
        public CleaningOrder CleaningOrder { get; set; } = null!;
    }
}
