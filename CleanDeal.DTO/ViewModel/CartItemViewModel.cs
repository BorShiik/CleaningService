using System.ComponentModel.DataAnnotations;
using CleanDeal.Model.Models;

namespace CleanDeal.DTO.ViewModel
{
    public class CartItemViewModel
    {
        public Product Product { get; set; } = null!;
        [Range(0, 50)]
        public int Quantity { get; set; }
    }
}