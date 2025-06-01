namespace CleanDeal.DTOs
{
    public class OrderListDTO
    {
        public int Id { get; set; }
        public string ServiceTypeName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}