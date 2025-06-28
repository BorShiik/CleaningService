using CleanDeal.Repositories;
using CleanDeal.Services.Loyalty;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CleanDeal.ViewModel;
using CleanDeal.Models;

namespace CleanDeal.Controllers
{
    public class ServicesController : Controller
    {
        private readonly IServiceTypeRepository _serviceRepo;
        private readonly IServicePackageRepository _packageRepo;
        private readonly IUserDiscountRepository _discountRepo;
        private readonly ILoyaltyService _loyalty;

        public ServicesController(IServiceTypeRepository serviceRepo,
                                   IServicePackageRepository packageRepo,
                                   IUserDiscountRepository discountRepo,
                                   ILoyaltyService loyalty)
        {
            _serviceRepo = serviceRepo;
            _packageRepo = packageRepo;
            _discountRepo = discountRepo;
            _loyalty = loyalty;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var services = await _serviceRepo.GetAllAsync();
            var packages = await _packageRepo.GetAllAsync();

            int points = 0;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                points = await _loyalty.GetBalanceAsync(userId);
            }

            var vm = new ServiceIndexViewModel
            {
                ServiceTypes = services,
                Packages = packages,
                LoyaltyPoints = points
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> PurchaseDiscount(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var balance = await _loyalty.GetBalanceAsync(userId);
            const int cost = 100;
            if (balance < cost)
            {
                TempData["Error"] = "Brak wystarczającej liczby punktów.";
                return RedirectToAction(nameof(Index));
            }

            await _loyalty.AwardPointsAsync(userId, -cost, $"Discount for service {id}");
            await _discountRepo.AddAsync(new UserServiceDiscount
            {
                UserId = userId,
                ServiceTypeId = id,
                Percentage = 10,
                Redeemed = false
            });

            TempData["Message"] = "Zakupiono zniżkę na usługę.";
            return RedirectToAction(nameof(Index));
        }
    }
}
