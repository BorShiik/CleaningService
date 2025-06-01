namespace CleanDeal.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.Now;

        public int CleaningOrderId { get; set; }
        public CleaningOrder CleaningOrder { get; set; } = null!;

        public string SenderId { get; set; } = null!;
        public ApplicationUser Sender{ get; set; } = null!;

        public string? ReceiverId { get; set; }
        public ApplicationUser? Receiver { get; set; }
    }
}
