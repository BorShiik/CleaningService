using CleanDeal.Models;

namespace CleanDeal.ViewModel
{
    public class CartItemViewModel
    {
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
    }
}