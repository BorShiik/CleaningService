using CleanDeal.Models.Email;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;

namespace CleanDeal.Services.Email;

public class ResendEmailSender : IEmailSender
{
    private readonly HttpClient _http;
    private readonly EmailSettings _cfg;

    public ResendEmailSender(IHttpClientFactory http,
                             IOptions<EmailSettings> cfg)
    {
        _cfg = cfg.Value;
        _http = http.CreateClient("Resend");

        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _cfg.Resend.ApiKey);
    }

    public async Task SendAsync(string to, string subject, string html,
                                CancellationToken ct = default)
    {
        var from = _cfg.Resend.From ?? $"{_cfg.FromName} <{_cfg.FromEmail}>";

        var payload = new
        {
            from,
            to = new[] { to },
            subject,
            html
        };

        var response = await _http.PostAsJsonAsync("emails", payload, ct);
        if (!response.IsSuccessStatusCode)
        {
            var msg = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException(
                $"Resend API error {(int)response.StatusCode}: {msg}");
        }
    }
}
