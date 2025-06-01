using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CleanDeal.ViewModel;

/// <summary>
/// Formularz tworzenia / edycji zlecenia (wiele usług jednocześnie).
/// </summary>
public class OrderCreateViewModel
{
    /// <summary>Potrzebne przy edycji.</summary>
    public int Id { get; set; }

    [Required(ErrorMessage = "Podaj datę sprzątania")]
    [Display(Name = "Termin sprzątania")]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.Today.AddDays(1);

    [Required, StringLength(200, ErrorMessage = "Maks. 200 znaków")]
    [Display(Name = "Adres")]
    public string Address { get; set; } = string.Empty;

    /// <summary>Id-ki zaznaczonych usług (przychodzą z formularza).</summary>
    [Required(ErrorMessage = "Wybierz co najmniej jedną usługę.")]
    public List<int> SelectedServiceTypeIds { get; set; } = new();

    /// <summary>Lista do wygenerowania checkboxów.</summary>
    public IEnumerable<SelectListItem> ServiceTypeOptions { get; set; } = Array.Empty<SelectListItem>();

    /// <summary>Mapa Id → Cena (wykorzystuje ją skrypt JS).</summary>
    public Dictionary<int, decimal> PriceMap { get; set; } = new();

    /// <summary>Łączna cena zamówienia (uzupełnia JS; przy edycji wczytujemy z bazy).</summary>
    public decimal TotalPrice { get; set; }
}
