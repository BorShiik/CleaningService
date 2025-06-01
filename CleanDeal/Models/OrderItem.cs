namespace CleanDeal.Models;

public class OrderItem
{
    public int Id { get; set; }

    public int ServiceTypeId { get; set; }
    public ServiceType ServiceType { get; set; } = null!;

    public int CleaningOrderId { get; set; }
    public CleaningOrder CleaningOrder { get; set; } = null!;

    public decimal Price { get; set; }          // ⬅️ BRAKOWAŁO
}
