using CleanDeal.DTOs;

namespace CleanDeal.ViewModel
{
    public class AdminDashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<CleaningOrderDTO> RecentOrders { get; set; } = new List<CleaningOrderDTO>();
    }
}
