using CleanDeal.DTOs;

namespace CleanDeal.ViewModel
{
    public class AdminDashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public double AverageOrderRating { get; set; }
        public List<CleanerRatingDTO> CleanerRatings { get; set; } = new();
        public decimal TotalRevenue { get; set; }
        public List<CleaningOrderDTO> RecentOrders { get; set; } = new List<CleaningOrderDTO>();
    }
}
