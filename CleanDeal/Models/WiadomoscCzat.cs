namespace CleanDeal.Models
{
    public class WiadomoscCzat
    {
        public int Id { get; set; }
        public int ZamowienieSprzataniaId { get; set; }
        public ZamowienieSprzatania ZamowienieSprzatania { get; set; }
        public string NadawcaId { get; set; }
        public Uzytkownik Nadawca { get; set; }
        public string Tresc {  get; set; }
        public DateTime DataWyslania { get; set; }
    }
}
