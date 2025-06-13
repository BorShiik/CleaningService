using CleanDeal.Repositories;
using CleanDeal.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace CleanDeal.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly IConfiguration _cfg;
        private const string CartKey = "cart";

        public CartController(IProductRepository productRepo, IConfiguration cfg)
        {
            _productRepo = productRepo;
            _cfg = cfg;
        }

        public async Task<IActionResult> Index()
        {
            var cart = GetCart();
            var items = new List<CartItemViewModel>();
            foreach (var kv in cart)
            {
                var product = await _productRepo.GetByIdAsync(kv.Key);
                if (product != null)
                {
                    items.Add(new CartItemViewModel
                    {
                        Product = product,
                        Quantity = kv.Value
                    });
                }
            }
            ViewBag.Total = items.Sum(i => i.Product.Price * i.Quantity);
            return View(items);
        }

        [HttpPost]
        public IActionResult Add(int id, int qty = 1)
        {
            var cart = GetCart();
            if (cart.ContainsKey(id))
                cart[id] += qty;
            else
                cart[id] = qty;
            SaveCart(cart);
            return RedirectToAction("Index", "Products");
        }

        [HttpPost]
        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            if (cart.Remove(id))
                SaveCart(cart);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                TempData["Error"] = "Koszyk jest pusty.";
                return RedirectToAction(nameof(Index));
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card", "blik", "p24" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = Url.Action(nameof(Success), "Cart", null, Request.Scheme) + "?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = Url.Action(nameof(Index), "Cart", null, Request.Scheme)
            };

            foreach (var kv in cart)
            {
                var product = await _productRepo.GetByIdAsync(kv.Key);
                if (product == null) continue;
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(product.Price * 100),
                        Currency = "pln",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = product.Name
                        }
                    },
                    Quantity = kv.Value
                });
            }

            var session = await new SessionService().CreateAsync(options);
            return Json(new { id = session.Id, key = _cfg["Stripe:PublishableKey"] });
        }

        public IActionResult Success()
        {
            HttpContext.Session.Remove(CartKey);
            TempData["Message"] = "Płatność zakończona pomyślnie.";
            return RedirectToAction("Index", "Products");
        }

        private Dictionary<int, int> GetCart()
        {
            var json = HttpContext.Session.GetString(CartKey);
            if (json == null) return new Dictionary<int, int>();
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, int>>(json) ?? new Dictionary<int, int>();
        }

        private void SaveCart(Dictionary<int, int> cart)
        {
            HttpContext.Session.SetString(CartKey, System.Text.Json.JsonSerializer.Serialize(cart));
        }
    }
}