namespace CleanDeal.DTO.DTOs
{
    public class ReviewDTO
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CleaningOrderId { get; set; }
        public string? UserFullName { get; set; }
        public string UserID { get; set; } = null!;
    }
}
