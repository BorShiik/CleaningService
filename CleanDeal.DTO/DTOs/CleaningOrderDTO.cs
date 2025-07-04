using CleanDeal.Model.Models;

namespace CleanDeal.DTO.DTOs
{
    public class CleaningOrderDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Address { get; set; } = string.Empty;
        public ApplicationUser? Cleaner { get; set; }
        public bool IsCompleted { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ServiceNames { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public bool IsPaid => PaymentAmount.HasValue;
        public decimal? PaymentAmount { get; set; }
        public decimal? TipAmount { get; set; }
        public decimal? TotalPrice { get; set; }
        public bool HasReview { get; set; }
        public int? ReviewRating { get; set; }
        public int LoyaltyPoints { get; set; }
    }
}
