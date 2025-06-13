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

        public StripeWebhookController(IConfiguration cfg, IPaymentRepository payRepo)
        {
            _cfg = cfg;
            _payRepo = payRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Handle()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            var sig = Request.Headers["Stripe-Signature"];
            var secret = _cfg["Stripe:WebhookSecret"];    

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, sig, secret);
            }
            catch (Exception)
            {
                return BadRequest(); 
            }

            if (stripeEvent.Type == Events.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Session;
                var orderId = int.Parse(session!.Metadata["orderId"]);

                if (await _payRepo.GetByOrderIdAsync(orderId) == null)
                {
                    await _payRepo.AddAsync(new Payment
                    {
                        Amount = (session.AmountTotal ?? 0) / 100m,
                        PaymentDate = DateTime.UtcNow,
                        CleaningOrderId = orderId
                    });
                }
            }
            return Ok();
        }
    }
}
