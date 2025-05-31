namespace CleanDeal.DTOs
{
    public class CleaningOrderDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Address { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }

        public string ServiceTypeName { get; set; } = string.Empty;  
        public string UserEmail { get; set; } = string.Empty;
        public bool IsPaid => PaymentAmount.HasValue;
        public decimal? PaymentAmount { get; set; }
        public bool HasReview { get; set; }
        public int? ReviewRating { get; set; }
    }
}
