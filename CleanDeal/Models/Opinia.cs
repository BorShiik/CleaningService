using Microsoft.Identity.Client;

namespace CleanDeal.Models
{
    public class Opinia
    {
        public int Id { get; set; }
        public int ZamowienieSprzataniaId { get; set; }
        public ZamowienieSprzatania ZamowienieSprzatania { get; set; }
        public int Ocena {  get; set; }
        public string Komentarz {  get; set; }
        public DateTime DataDodania { get; set; }
    }
}
