namespace CleanDeal.Models
{
    public class Dostepnosc
    {
        public int Id { get; set; }
        public string SprzataczId { get; set; }
        public Uzytkownik Sprzatacz { get; set; }
        public DayOfWeek DzienTygodnia { get; set; }
        public TimeSpan GodzinaOd {  get; set; }
        public TimeSpan GodzinaDo {  get; set; }
    }
}
