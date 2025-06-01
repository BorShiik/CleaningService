namespace CleanDeal.DTOs
{
    public class ChatMessageDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public string UserName { get; set; } = default!;      
        public string Content { get; set; } = default!;
        public DateTime SentAt { get; set; }
        public int CleaningOrderId { get; set; }
    }
}
