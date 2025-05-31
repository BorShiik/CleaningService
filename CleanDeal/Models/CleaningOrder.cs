using System.ComponentModel.DataAnnotations;

namespace CleanDeal.Models
{
    public class CleaningOrder
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required, StringLength(200)]
        public string Address { get; set; }

        public bool IsCompleted { get; set; }

        public int ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public Payment? Payment { get; set; }
        public Review? Review { get; set; }

        public ICollection<ChatMessage>? ChatMessages { get; set; }
    }
}
