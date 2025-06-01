namespace CleanDeal.DTOs
{
    public class ChatMessageDTO
    {
        public int Id { get; set; }
        public int CleaningOrderId { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string? ReceiverId { get; set; }
        public string? ReceiverName { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}