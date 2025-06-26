using CleanDeal.Models;
using CleanDeal.Repositories;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace CleanDeal.Controllers
{
    [ApiController]
    [Route("webhooks/stripe")]
    public class StripeWebhookController : Controller
    {
        private readonly IConfiguration _cfg;
        private readonly IPaymentRepository _payRepo;
        private readonly ICleaningOrderRepository _cleanRepo;
        private readonly IProductOrderRepository _prodRepo;
        private readonly IProductRepository _productRepo;

        public StripeWebhookController(
            IConfiguration cfg,
            IPaymentRepository payRepo,
            ICleaningOrderRepository cleanRepo,
            IProductOrderRepository prodRepo,
            IProductRepository productRepo)
        {
            _cfg = cfg;
            _payRepo = payRepo;
            _cleanRepo = cleanRepo;
            _prodRepo = prodRepo;
            _productRepo = productRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Handle()
        {
            var payload = await new StreamReader(Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];
            var secret = _cfg["Stripe:WebhookSecret"];

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(payload, signature, secret);
            }
            catch
            {
                return BadRequest();
            }

            if (stripeEvent.Type == Events.CheckoutSessionCompleted)
            {
                var session = (Session)stripeEvent.Data.Object;
                var orderId = int.Parse(session.Metadata["orderId"]);
                var orderType = session.Metadata.TryGetValue("orderType", out var t)
                    ? t : "cleaning";
                session.Metadata.TryGetValue("tip", out var tipStr);
                decimal tip = 0m;

                decimal.TryParse(tipStr, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out tip);


                var existing = await _payRepo.GetByOrderIdAsync(orderId);

                if (orderType == "product" && existing != null)
                    return Ok();


                if (orderType.StartsWith("cleaning") && await _cleanRepo.GetByIdAsync(orderId) == null)
                    return BadRequest("Cleaning order not found");
                if (orderType == "product" && await _prodRepo.GetByIdAsync(orderId) == null)
                    return BadRequest("Product order not found");




                if (orderType == "product")
                {
                    var payment = new Payment
                    {
                        Amount = (session.AmountTotal ?? 0) / 100m,
                        PaymentDate = DateTime.UtcNow,
                        ProductOrderId = orderId
                    };
                    await _payRepo.AddAsync(payment);

                    var order = await _prodRepo.GetByIdAsync(orderId);
                    if (order != null)
                    {
                        foreach (var item in order.Items)
                        {
                            await _productRepo.DecreaseStockAsync(item.ProductId, item.Quantity);
                        }
                    }
                }
                else
                {
                    if (existing == null)
                    {
                        var payment = new Payment
                        {
                            Amount = ((session.AmountTotal ?? 0) / 100m) - tip,
                            Tip = tip,
                            PaymentDate = DateTime.UtcNow,
                            CleaningOrderId = orderId
                        };
                        await _payRepo.AddAsync(payment);
                    }
                    else if (tip > 0)
                    {
                        existing.Tip += tip;
                        await _payRepo.UpdateAsync(existing);
                    }
                }
            }

                return Ok();
            }
        }
    }

