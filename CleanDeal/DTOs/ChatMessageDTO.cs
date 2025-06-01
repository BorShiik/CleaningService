namespace CleanDeal.DTOs
{
    public class ChatMessageDTO
    {
        public int Id { get; set; }
        public string SenderId { get; set; } = null!;
        public string SenderName { get; set; } = null!;
        public string ReceiverId { get; set; } = null!;
        public string ReceiverName { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; }
        public int CleaningOrderId { get; set; }
    }
}
