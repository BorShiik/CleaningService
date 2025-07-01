namespace CleanDeal.Model.Models
{
    public class CleaningOrderService
    {
        public int Id { get; set; }
        public int CleaningOrderId { get; set; }
        public CleaningOrder CleaningOrder { get; set; } = null!;
        public int ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; } = null!;
    }
}