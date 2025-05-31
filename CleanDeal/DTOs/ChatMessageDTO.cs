namespace CleanDeal.DTOs
{
    public class ChatMessageDTO
    {
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public string SenderName { get; set; } = string.Empty;
    }
}
