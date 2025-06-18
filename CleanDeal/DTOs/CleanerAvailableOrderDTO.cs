namespace CleanDeal.DTOs
{
    public class CleanerAvailableOrderDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Address { get; set; } = string.Empty;
        public string ServiceNames { get; set; } = string.Empty;
    }
}
