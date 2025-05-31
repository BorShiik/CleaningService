using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleanDeal.ViewModel
{
    public class OrderCreateViewModel
    {
        public DateTime Date { get; set; }
        public string Address { get; set; } = string.Empty;
        public int ServiceTypeId { get; set; }
        public IEnumerable<SelectListItem>? ServiceTypeOptions { get; set; }
    }
}
