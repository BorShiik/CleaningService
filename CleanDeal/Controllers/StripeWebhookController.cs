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

                
                if (await _payRepo.GetByOrderIdAsync(orderId) != null)
                    return Ok();

                var payment = new Payment
                {
                    Amount = (session.AmountTotal ?? 0) / 100m,
                    PaymentDate = DateTime.UtcNow,
                    CleaningOrderId = orderType == "cleaning" ? orderId : null,
                    ProductOrderId = orderType == "product" ? orderId : null
                };

                
                if (orderType == "cleaning" && await _cleanRepo.GetByIdAsync(orderId) == null)
                    return BadRequest("Cleaning order not found");
                if (orderType == "product" && await _prodRepo.GetByIdAsync(orderId) == null)
                    return BadRequest("Product order not found");

                await _payRepo.AddAsync(payment);


                if (orderType == "product")
                {
                    var order = await _prodRepo.GetByIdAsync(orderId);
                    if (order != null)
                    {
                        foreach (var item in order.Items)
                        {
                            await _productRepo.DecreaseStockAsync(item.ProductId, item.Quantity);
                        }
                    }
                }
            }

            return Ok();
        }
    }
}
