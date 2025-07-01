namespace CleanDeal.Model.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public decimal Tip { get; set; }
        public DateTime PaymentDate { get; set; }
        public int? CleaningOrderId { get; set; }
        public CleaningOrder? CleaningOrder { get; set; }
        public int? ProductOrderId { get; set; }
        public ProductOrder? ProductOrder { get; set; }
    }
}
