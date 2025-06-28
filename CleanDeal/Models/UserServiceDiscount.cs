namespace CleanDeal.Models
{
    public class UserServiceDiscount
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public int ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; } = null!;
        public decimal Percentage { get; set; }
        public bool Redeemed { get; set; }
    }
}