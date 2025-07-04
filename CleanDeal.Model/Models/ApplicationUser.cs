using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CleanDeal.Model.Models
{
    public enum Gender
    {
        Mężczyzna,
        Kobieta
    }
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;
        public ICollection<CleaningOrder>? CleaningOrders { get; set; }
        public ICollection<ProductOrder>? ProductOrders { get; set; }
        public ICollection<CleaningOrder> CleanerOrders { get; set; } = new List<CleaningOrder>();
        public ICollection<ChatMessage>? SentMessages { get; set; }
        public ICollection<ChatMessage>? ReceivedMessages { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public int LoyaltyPoints {  get; set; }
        public ICollection<LoyaltyTransaction>? LoyaltyTransactions { get; set; }
        public byte[]? Avatar { get; set; }
        public Gender? Gender { get; set; }

    }
}
