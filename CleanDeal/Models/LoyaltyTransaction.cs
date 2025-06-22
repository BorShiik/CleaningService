using System;
using System.ComponentModel.DataAnnotations;

namespace CleanDeal.Models
{
    public class LoyaltyTransaction
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null;

        public int Points { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
