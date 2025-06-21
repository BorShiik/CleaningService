using CleanDeal.Services.Email;
using CleanDeal.Models;
using Microsoft.AspNetCore.Mvc;

namespace CleanDeal.Services.Notifications
{
    public class EmailNotificationService
    {
        private readonly IEmailSender _sender;
        private readonly TemplateRenderer _tpl;

        public EmailNotificationService(IEmailSender sender, TemplateRenderer tpl)
        {
            _sender = sender;
            _tpl = tpl;
        }

        public Task SendRegistrationAsync(ApplicationUser user, string confirmUrl, CancellationToken ct = default)
        => SendByTemplate("RegistrationConfirmation",
                          user.Email!,
                          "Registration Confirmation",
                          new { ConfirmUrl = confirmUrl }, ct);

        public Task SendOrderCreatedAsync(CleaningOrder order, CancellationToken ct = default)
        => SendByTemplate("OrderCreated",
                          order.User.Email!,
                          $"Your order #{order.Id} has been created",
                          order, ct);

        public Task SendOrderAcceptedAsync(CleaningOrder order, CancellationToken ct = default)
            => SendByTemplate("OrderAccepted",
                              order.User.Email!,
                              $"Your order #{order.Id} has been accepted",
                              new
                              {
                                  order.Id,
                                  order.Date,
                                  Cleaner = order.Cleaner!.FullName
                              }, ct);

        public Task SendOrderCompletedAsync(CleaningOrder order, string reviewUrl, CancellationToken ct = default)
            => SendByTemplate("OrderCompleted",
                              order.User.Email!,
                              $"Your order #{order.Id} has been finished",
                              new { order.Id, reviewUrl }, ct);

        private async Task SendByTemplate<T>(string view, string to, string subject, T model, CancellationToken ct)
        {
            string html = await _tpl.RenderAsync(view, model);
            await _sender.SendAsync(to, subject, html, ct);
        }
    }
}
