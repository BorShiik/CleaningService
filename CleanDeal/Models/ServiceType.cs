namespace CleanDeal.Models
{
    public class ServiceType
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<CleaningOrder>? CleaningOrders { get; set; }
    }
}
