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
        private readonly ILoyaltyService _loyalty;

        public ServicesController(IServiceTypeRepository serviceRepo,
                                   IServicePackageRepository packageRepo,
                                   ILoyaltyService loyalty)
        {
            _serviceRepo = serviceRepo;
            _packageRepo = packageRepo;
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
        public async Task<IActionResult> PurchaseDiscount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var balance = await _loyalty.GetBalanceAsync(userId);
            const int cost = 100;
            if (balance < cost)
            {
                return BadRequest("Brak wystarczającej liczby punktów.");
            }

            HttpContext.Session.SetInt32("ServiceDiscount", 1);
            return Ok();
        }
    }
}
