using System.Security.Claims;
using CleanDeal.Models;
using CleanDeal.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CleanDeal.Areas.Identity.Pages.Account.Manage;

[Authorize(Roles = "Cleaner")]
public class EarningsModel : PageModel
{
    private readonly ICleaningOrderRepository _orderRepo;

    public record OrderRow(int OrderId, DateTime Date, decimal Amount, decimal Tip)
    {
        public decimal Earning => Amount * 0.8m + Tip;
    }

    public List<OrderRow> Orders { get; set; } = new();
    public decimal TotalEarnings { get; set; }

    public EarningsModel(ICleaningOrderRepository orderRepo)
    {
        _orderRepo = orderRepo;
    }

    public async Task OnGetAsync()
    {
        var cleanerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var orders = await _orderRepo.GetCompletedByCleanerAsync(cleanerId);
        foreach (var o in orders)
        {
            var amount = o.Payment?.Amount ?? 0m;
            var tip = o.Payment?.Tip ?? 0m;
            Orders.Add(new OrderRow(o.Id, o.Date, amount, tip));
        }
        TotalEarnings = Orders.Sum(o => o.Earning);
    }
}