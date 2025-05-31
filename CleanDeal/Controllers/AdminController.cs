using AutoMapper;
using CleanDeal.DTOs;
using CleanDeal.Repositories;
using CleanDeal.ViewModel;
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
        private readonly IMapper _mapper;

        public AdminController(ICleaningOrderRepository orderRepo, IPaymentRepository paymentRepo, IApplicationUserRepository userRepo, IMapper mapper)
        {
            _orderRepo = orderRepo;
            _paymentRepo = paymentRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalOrders = await _orderRepo.CountAsync();
            var totalUsers = await _userRepo.CountAsync();
            decimal totalRevenue = 0;
            var payments = await _paymentRepo.GetAllAsync();
            if (payments != null)
            {
                totalRevenue = payments.Sum(p => p.Amount);
            }
            
            var recentOrdersDomain = await _orderRepo.GetRecentOrdersAsync(5);
            var recentOrdersDto = _mapper.Map<List<CleaningOrderDTO>>(recentOrdersDomain);

            var dashboardVm = new AdminDashboardViewModel
            {
                TotalOrders = totalOrders,
                TotalUsers = totalUsers,
                TotalRevenue = totalRevenue,
                RecentOrders = recentOrdersDto
            };

            return View(dashboardVm);
        }
    }
}
