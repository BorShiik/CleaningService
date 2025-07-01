namespace CleanDeal.DTO.DTOs
{
    public class PaymentDTO
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public decimal Tip { get; set; }
        public DateTime PaymentDate { get; set; }
        public int? CleaningOrderId { get; set; }
    }
}
