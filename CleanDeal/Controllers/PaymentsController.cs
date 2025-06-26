using AutoMapper;
using CleanDeal.DTOs;
using CleanDeal.Models;
using CleanDeal.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        private readonly IMapper _mapper;
        public PaymentsController(IConfiguration cfg, ICleaningOrderRepository orderRepo, IPaymentRepository paymentRepo, IMapper mapper)
        {
            _cfg = cfg;
            _orderRepo = orderRepo;
            _paymentRepo = paymentRepo;
            _mapper = mapper;
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
            ViewBag.AmountDisplay = GetAmountInZloty(order);

            var dto = _mapper.Map<CleaningOrderDTO>(order);

            return View(dto);    // Views/Payments/Checkout.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckoutSession(int orderId, decimal tip = 0)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return NotFound();

            long amount = GetAmountInCents(order);
            long tipCents = (long)Math.Round(tip * 100m);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card", "blik", "p24" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount  = amount,
                            Currency    = "pln",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"Usługa sprzątania #{order.Id}"
                            }

                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = Url.Action("Success", "Payments",
                    new { orderId, tip }, Request.Scheme) + "&session_id ={ CHECKOUT_SESSION_ID }",
                CancelUrl = Url.Action("Cancel", "Payments",
                    new { orderId }, Request.Scheme),
                Metadata = new Dictionary<string, string>
    {
                { "orderId",  orderId.ToString() },
                { "orderType", "cleaning" },
                { "tip", tip.ToString(System.Globalization.CultureInfo.InvariantCulture) }
            }
            };

            if (tipCents > 0)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = tipCents,
                        Currency = "pln",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Napiwek dla sprzątacza"
                        }
                    },
                    Quantity = 1
                });
            }

            var session = await new SessionService().CreateAsync(options);
            return Json(new { id = session.Id });
        }

        public async Task<IActionResult> Tip(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && order.UserId != userId)
                return Forbid();

            if (order.Payment == null)
            {
                TempData["Error"] = "Najpierw opłać zamówienie.";
                return RedirectToAction("Details", "Orders", new { id });
            }

            if (order.Status != OrderStatus.Finished)
            {
                TempData["Error"] = "Napiwek można dodać po zakończeniu zlecenia.";
                return RedirectToAction("Details", "Orders", new { id });
            }

            ViewBag.PublishableKey = _cfg["Stripe:PublishableKey"];
            var dto = _mapper.Map<CleaningOrderDTO>(order);
            return View("Tip", dto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTipSession(int orderId, decimal tip)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null || order.Payment == null) return NotFound();

            long tipCents = (long)Math.Round(tip * 100m);
            if (tipCents <= 0) return BadRequest();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card", "blik", "p24" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = tipCents,
                            Currency = "pln",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Napiwek dla sprzątacza"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = Url.Action("Success", "Payments",
                    new { orderId, tip }, Request.Scheme) + "&session_id ={ CHECKOUT_SESSION_ID }",
                CancelUrl = Url.Action("Cancel", "Payments",
                    new { orderId }, Request.Scheme),
                Metadata = new Dictionary<string, string>
                {
                    { "orderId", orderId.ToString() },
                    { "orderType", "cleaning_tip" },
                    { "tip", tip.ToString(System.Globalization.CultureInfo.InvariantCulture) }
                }
            };

            var session = await new SessionService().CreateAsync(options);
            return Json(new { id = session.Id });
        }

        public async Task<IActionResult> Success(int orderId, decimal? tip)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return NotFound();

            if (order.Payment == null)       
            {
                await _paymentRepo.AddAsync(new Payment
                {
                    Amount = GetAmountInZloty(order),
                    Tip = tip ?? 0,
                    PaymentDate = DateTime.UtcNow,
                    CleaningOrderId = orderId
                });
            }
            else if (tip.HasValue && tip.Value > 0)
            {
                order.Payment.Tip += tip.Value;
                await _paymentRepo.UpdateAsync(order.Payment);
            }

            TempData["Message"] = "Płatność zakończona pomyślnie.";
            return RedirectToAction("Details", "Orders", new { id = orderId });
        }

        public IActionResult Cancel(int orderId)
        {
            TempData["Error"] = "Płatność została anulowana.";
            return RedirectToAction("Details", "Orders", new { id = orderId });
        }

        private static long GetAmountInCents(CleaningOrder order)
            => (long)Math.Round((order.TotalPrice ?? 0m) * 100m);   

        private static decimal GetAmountInZloty(CleaningOrder order)
            => order.TotalPrice ?? 0m;
    }
}

