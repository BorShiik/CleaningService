using Microsoft.AspNetCore.Identity;

namespace CleanDeal.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public ICollection<CleaningOrder>? CleaningOrders { get; set; }
        public ICollection<ChatMessage>? ChatMessages { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
