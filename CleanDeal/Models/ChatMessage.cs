namespace CleanDeal.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.Now;

        public int CleaningOrderId { get; set; }
        public CleaningOrder CleaningOrder { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
