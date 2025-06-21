namespace CleanDeal.DTOs
{
    public class CleanerMyOrderDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Address { get; set; } = string.Empty;
        public string ServiceNames { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;   
        public bool CanComplete { get; set; }
    }
}
