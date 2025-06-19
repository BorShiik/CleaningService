using CleanDeal.Models;

namespace CleanDeal.DTOs
{
    public class ChatMessageDTO
    {
        public int Id { get; set; }
        public int CleaningOrderId { get; set; }
        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }
        public bool IsAdmin { get; set; }
        public string? ReceiverId { get; set; }
        public ApplicationUser? Receiver { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}