using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanDeal.Models;

/// <summary>Status zlecenia – identyczny jak dotąd, tylko dodaliśmy TotalPrice i Items.</summary>
public enum OrderStatus { WaitingForCleaner = 0, InProcess = 1, Finished = 2 }

public class CleaningOrder
{
    public int Id { get; set; }

    [Required] public DateTime Date { get; set; }
    [Required, StringLength(200)] public string Address { get; set; } = string.Empty;

    /* ---------------------- nowe / przeniesione pola ---------------------- */
    [Range(0, 10_000)] public decimal TotalPrice { get; set; }      // suma pozycji
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    /* --------------------------- relacje EF --------------------------- */
    public OrderStatus Status { get; set; } = OrderStatus.WaitingForCleaner;

    public int ServiceTypeId { get; set; }
    public ServiceType ServiceType { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public string? CleanerId { get; set; }
    public ApplicationUser? Cleaner { get; set; }

    public Payment? Payment { get; set; }
    public Review? Review { get; set; }
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    [NotMapped]
    public bool IsCompleted => Status == OrderStatus.Finished;
}
