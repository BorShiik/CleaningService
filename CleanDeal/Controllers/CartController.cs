// Controllers/CartController.cs
using CleanDeal.Models;
using CleanDeal.Repositories;
using CleanDeal.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace CleanDeal.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly IProductOrderRepository _orderRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IConfiguration _cfg;

        private const string CartKey = "cart";   

        public CartController(IProductRepository productRepo,
                              IPaymentRepository paymentRepo,
                              IConfiguration cfg,
                              IProductOrderRepository orderRepo)
        {
            _productRepo = productRepo;
            _paymentRepo = paymentRepo;
            _cfg = cfg;
            _orderRepo = orderRepo;
        }

        

        public async Task<IActionResult> Index()
        {
            var cart = GetCart();
            var items = new List<CartItemViewModel>();

            foreach (var (productId, qty) in cart)
            {
                var product = await _productRepo.GetByIdAsync(productId);
                if (product != null)
                    items.Add(new CartItemViewModel { Product = product, Quantity = qty });
            }

            ViewBag.Total = items.Sum(i => i.Product.Price * i.Quantity);
            return View(items);
        }

      

        [HttpPost]
        public async Task<IActionResult> Add(int id, int qty = 1)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null || product.StockQuantity <= 0)
            {
                TempData["Error"] = "Produkt niedostępny.";
                return RedirectToAction("Index", "Products");
            }

            var cart = GetCart();
            var current = cart.TryGetValue(id, out var q) ? q : 0;
            if (current + qty > product.StockQuantity)
            {
                TempData["Error"] = "Brak wystarczającej ilości produktu w magazynie.";
                qty = product.StockQuantity - current;
                if (qty <= 0) return RedirectToAction("Index", "Products");
            }

            cart[id] = current + qty;
            SaveCart(cart);
            return RedirectToAction("Index", "Products");
        }

        [HttpPost]
        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            if (cart.Remove(id)) SaveCart(cart);
            return RedirectToAction(nameof(Index));
        }

        

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                TempData["Error"] = "Koszyk jest pusty.";
                return RedirectToAction(nameof(Index));
            }

            var items = new List<CartItemViewModel>();
            foreach (var (productId, qty) in cart)
            {
                var product = await _productRepo.GetByIdAsync(productId);
                if (product != null) items.Add(new CartItemViewModel { Product = product, Quantity = qty });
            }

            ViewBag.Total = items.Sum(i => i.Product.Price * i.Quantity);
            ViewBag.Items = items;
            return View(new ProductOrderCreateViewModel());
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(ProductOrderCreateViewModel model)
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                TempData["Error"] = "Koszyk jest pusty.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await RepopulateViewBagAsync(cart);
                return View(model);
            }

            foreach (var (productId, qty) in cart)
            {
                var product = await _productRepo.GetByIdAsync(productId);
                if (product == null || product.StockQuantity < qty)
                {
                    ModelState.AddModelError(string.Empty, $"Produkt {product?.Name ?? productId.ToString()} ma tylko {product?.StockQuantity ?? 0} sztuk.");
                    await RepopulateViewBagAsync(cart);
                    return View(model);
                }
            }

            var order = new ProductOrder
            {
                OrderDate = DateTime.UtcNow,
                Address = model.Address,
                DeliveryMethod = model.DeliveryMethod,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            foreach (var (productId, qty) in cart)
                order.Items.Add(new ProductOrderItem { ProductId = productId, Quantity = qty });

            await _orderRepo.AddAsync(order);                 

            
            
            var options = BuildStripeOptions(order);
            var session = await new SessionService().CreateAsync(options);

            await _orderRepo.UpdateAsync(order);

            
            HttpContext.Session.Remove(CartKey);
            return Redirect(session.Url);   
        }

       

        public async Task<IActionResult> Success(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order is { Payment: null })
            {
                var amount = order.Items.Sum(i => i.Product.Price * i.Quantity);
                await _paymentRepo.AddAsync(new Payment
                {
                    Amount = amount,
                    PaymentDate = DateTime.UtcNow,
                    ProductOrderId = order.Id
                });

                foreach (var item in order.Items)
                {
                    await _productRepo.DecreaseStockAsync(item.ProductId, item.Quantity);
                }
            }

            TempData["Message"] = "Płatność zakończona pomyślnie.";
            return RedirectToAction("Index", "Products");
        }

        public async Task<IActionResult> Cancel(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order != null) await _orderRepo.UpdateAsync(order);   
            TempData["Error"] = "Płatność została anulowana.";
            return RedirectToAction("Index", "Products");
        }


        private SessionCreateOptions BuildStripeOptions(ProductOrder order)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new() { "card", "blik", "p24" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = Url.Action(nameof(Success), "Cart",
                             new { orderId = order.Id }, Request.Scheme)
                             + "&session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = Url.Action(nameof(Cancel), "Cart",
                             new { orderId = order.Id }, Request.Scheme),

                
                Metadata = new()
        {
            { "orderId",   order.Id.ToString() },
            { "orderType", "product" }              
        }
            };

            foreach (var item in order.Items)
            {
                var product = item.Product ?? _productRepo.GetByIdAsync(item.ProductId).Result;
                if (product == null) continue;

                options.LineItems.Add(new SessionLineItemOptions
                {
                    Quantity = item.Quantity,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(product.Price * 100),
                        Currency = "pln",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = product.Name
                        }
                    }
                });
            }

            return options;
        }

        private Dictionary<int, int> GetCart()
        {
            var json = HttpContext.Session.GetString(CartKey);
            return json == null
                   ? new()
                   : System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, int>>(json)
                     ?? new();
        }

        private void SaveCart(Dictionary<int, int> cart)
            => HttpContext.Session.SetString(CartKey,
                   System.Text.Json.JsonSerializer.Serialize(cart));

        private async Task RepopulateViewBagAsync(Dictionary<int, int> cart)
        {
            var items = new List<CartItemViewModel>();
            foreach (var (productId, qty) in cart)
            {
                var product = await _productRepo.GetByIdAsync(productId);
                if (product != null) items.Add(new CartItemViewModel { Product = product, Quantity = qty });
            }
            ViewBag.Total = items.Sum(i => i.Product.Price * i.Quantity);
            ViewBag.Items = items;
        }
    }
}
