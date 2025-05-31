using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CleanDeal.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        public ICollection<CleaningOrder>? CleaningOrders { get; set; }
        public ICollection<ChatMessage>? ChatMessages { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
