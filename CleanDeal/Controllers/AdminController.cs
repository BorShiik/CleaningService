using AutoMapper;
using CleanDeal.DTO.DTOs;
using CleanDeal.DTO.ViewModel;
using CleanDeal.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanDeal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController: Controller
    {
        private readonly ICleaningOrderRepository _orderRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IApplicationUserRepository _userRepo;
        private readonly IProductRepository _productRepo;
        private readonly IReviewRepository _reviewRepo;
        private readonly IMapper _mapper;

        public AdminController(ICleaningOrderRepository orderRepo, IPaymentRepository paymentRepo,
            IApplicationUserRepository userRepo, IMapper mapper, IProductRepository productRepo, IReviewRepository reviewRepo )
        {
            _orderRepo = orderRepo;
            _paymentRepo = paymentRepo;
            _userRepo = userRepo;
            _mapper = mapper;
            _productRepo = productRepo;
            _reviewRepo = reviewRepo;
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalOrders = await _orderRepo.CountAsync();
            var totalUsers = await _userRepo.CountAsync();
            decimal totalRevenue = 0;
            var payments = await _paymentRepo.GetAllAsync();
            var totalProducts = await _productRepo.CountAsync();
            if (payments != null)
            {
                totalRevenue = payments.Sum(p => p.Amount);
            }

            var avgRating = await _reviewRepo.GetAverageRatingAsync();
            var cleanerRatings = (await _reviewRepo.GetAverageRatingByCleanerAsync()).ToList();
            var recentOrdersDomain = await _orderRepo.GetRecentOrdersAsync(5);
            var recentOrdersDto = _mapper.Map<List<CleaningOrderDTO>>(recentOrdersDomain);

            var dashboardVm = new AdminDashboardViewModel
            {
                TotalOrders = totalOrders,
                TotalUsers = totalUsers,
                TotalProducts = totalProducts,
                AverageOrderRating = avgRating,
                CleanerRatings = cleanerRatings,
                TotalRevenue = totalRevenue,
                RecentOrders = recentOrdersDto
            };

            return View(dashboardVm);
        }

        [HttpGet]
        public async Task<IActionResult> LoadOrders(int skip)
        {
            var orders = await _orderRepo.GetOrdersPagedAsync(skip, 5);
            var ordersDto = _mapper.Map<List<CleaningOrderDTO>>(orders);
            return PartialView("OrderRows", ordersDto);
        }
    }
}
