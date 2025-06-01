using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CleanDeal.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        public ICollection<CleaningOrder> ClientOrders { get; set; } = new List<CleaningOrder>();
        public ICollection<CleaningOrder> CleanerOrders { get; set; } = new List<CleaningOrder>();
        public ICollection<ChatMessage> SentMessages { get; set; } = new List<ChatMessage>();
        public ICollection<ChatMessage> ReceivedMessages { get; set; } = new List<ChatMessage>();
        public ICollection<Review>? Reviews { get; set; }
    }
}
