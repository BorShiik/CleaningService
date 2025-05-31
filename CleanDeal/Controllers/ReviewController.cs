using AutoMapper;
using CleanDeal.DTOs;
using CleanDeal.Models;
using CleanDeal.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanDeal.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepo;
        private readonly ICleaningOrderRepository _orderRepo;
        private readonly IMapper _mapper;

        public ReviewController(
            IReviewRepository reviewRepo,
            ICleaningOrderRepository orderRepo,
            IMapper mapper)
        {
            _reviewRepo = reviewRepo;
            _orderRepo = orderRepo;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && order.UserId != userId)
                return Forbid();

            if (!order.IsCompleted)
            {
                TempData["Error"] = "Najpierw oznacz zlecenie jako zakończone.";
                return RedirectToAction("Details", "Orders", new { id = orderId });
            }

            if (order.Review != null)
            {
                TempData["Error"] = "Recenzja dla tego zlecenia już istnieje.";
                return RedirectToAction("Details", "Orders", new { id = orderId });
            }

            ViewBag.OrderId = orderId;
            return View();                          
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int orderId, int rating, string? comment)
        {
            if (rating < 1 || rating > 5)
            {
                ModelState.AddModelError(nameof(rating), "Ocena musi być od 1 do 5.");
            }
            if (!ModelState.IsValid) return View();

            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && order.UserId != userId)
                return Forbid();

            await _reviewRepo.AddAsync(new Review
            {
                CleaningOrderId = orderId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            });

            TempData["Message"] = "Dziękujemy za wystawienie opinii!";
            return RedirectToAction("Details", "Orders", new { id = orderId });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> All()
        {
            var reviews = await _reviewRepo.GetAllAsync();
            var dto = _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
            return View(dto);                       // Views/Review/All.cshtml
        }
    }
}
