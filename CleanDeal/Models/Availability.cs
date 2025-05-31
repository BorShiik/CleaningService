namespace CleanDeal.Models
{
    public class Availability
    {
        public int Id { get; set; }
        public string SprzataczId { get; set; }
        public ApplicationUser Sprzatacz { get; set; }
        public DayOfWeek DzienTygodnia { get; set; }
        public TimeSpan GodzinaOd {  get; set; }
        public TimeSpan GodzinaDo {  get; set; }
    }
}
