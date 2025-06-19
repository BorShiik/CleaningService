using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CleanDeal.ViewModel
{
    public class OrderCreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Podaj datę i godzinę.")]
        [Display(Name = "Termin sprzątania")]
        public DateTime Date { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Adres")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Rodzaje usług")]
        [Required(ErrorMessage = "Wybierz przynajmniej jedną usługę.")]
        public List<int> ServiceTypeIds { get; set; } = new();

        [EmailAddress]
        [Display(Name = "Email klienta")]
        public string? UserEmail { get; set; }
        public IEnumerable<SelectListItem>? ServiceTypeOptions { get; set; }
    }
}
