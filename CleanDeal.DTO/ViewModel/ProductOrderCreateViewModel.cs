using System.ComponentModel.DataAnnotations;
using CleanDeal.Model.Models;

namespace CleanDeal.DTO.ViewModel
{
    public class ProductOrderCreateViewModel
    {
        [Required, StringLength(200)]
        [Display(Name = "Adres dostawy")]
        public string Address { get; set; } = string.Empty;
        [Display(Name = "Sposób dostawy")]
        [Required]
        public DeliveryMethod DeliveryMethod { get; set; }
    }
}