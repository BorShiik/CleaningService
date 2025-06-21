using CleanDeal.Models;
using System.ComponentModel.DataAnnotations;

namespace CleanDeal.ViewModel
{
    public class CartItemViewModel
    {
        public Product Product { get; set; } = null!;
        [Range(0, 50)]
        public int Quantity { get; set; }
    }
}