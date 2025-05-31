using CleanDeal.Models;
using CleanDeal.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace CleanDeal.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly IConfiguration _cfg;
        private readonly ICleaningOrderRepository _orderRepo;
        private readonly IPaymentRepository _paymentRepo;
        public PaymentsController(IConfiguration cfg, ICleaningOrderRepository orderRepo, IPaymentRepository paymentRepo)
        {
            _cfg = cfg;
            _orderRepo = orderRepo;
            _paymentRepo = paymentRepo;

            StripeConfiguration.ApiKey = _cfg["Stripe:SecretKey"];
        }

        public async Task<IActionResult> Checkout(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && order.UserId != userId)
                return Forbid();

            if (order.Payment != null)
            {
                TempData["Error"] = "To zlecenie jest już opłacone.";
                return RedirectToAction("Details", "Orders", new { id });
            }

            ViewBag.PublishableKey = _cfg["Stripe:PublishableKey"];
            ViewBag.OrderId = id;
            ViewBag.AmountDisplay = CalculateAmount(order) / 100m;

            return View(order);    // Views/Payments/Checkout.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckoutSession(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return NotFound();

            var amount = CalculateAmount(order);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount   = amount,
                            Currency     = "pln",
                            ProductData  = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"Usługa sprzątania #{order.Id}"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = Url.Action("Success", "Payments",
                           new { orderId }, Request.Scheme) + "?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = Url.Action("Cancel", "Payments",
                           new { orderId }, Request.Scheme)
            };

            var session = await new SessionService().CreateAsync(options);
            return Json(new { id = session.Id });
        }

        public async Task<IActionResult> Success(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return NotFound();

            if (order.Payment == null)       
            {
                await _paymentRepo.AddAsync(new Payment
                {
                    Amount = CalculateAmount(order) / 100m,
                    PaymentDate = DateTime.UtcNow,
                    CleaningOrderId = orderId
                });
            }

            TempData["Message"] = "Płatność zakończona pomyślnie.";
            return RedirectToAction("Details", "Orders", new { id = orderId });
        }

        public IActionResult Cancel(int orderId)
        {
            TempData["Error"] = "Płatność została anulowana.";
            return RedirectToAction("Details", "Orders", new { id = orderId });
        }

        private static long CalculateAmount(CleaningOrder order) =>
            order.ServiceTypeId switch
            {
                1 => 200_00,     
                2 => 150_00,
                3 => 100_00,
                _ => 180_00
            };
    }
}

