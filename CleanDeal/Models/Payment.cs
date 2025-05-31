namespace CleanDeal.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }

        public int CleaningOrderId { get; set; }
        public CleaningOrder CleaningOrder { get; set; } = null!;
    }
}
