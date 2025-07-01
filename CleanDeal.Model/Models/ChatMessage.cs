namespace CleanDeal.Model.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int CleaningOrderId { get; set; }
        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }
        public string? ReceiverId { get; set; }
        public ApplicationUser? Receiver { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public CleaningOrder CleaningOrder { get; set; }
    }
}