using System.Security.Claims;
using CleanDeal.Repositories.IRepositories;
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

    public double AverageRating { get; set; }
    public int FiveStarReviewsNeeded { get; set; }

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

        var ratings = orders
            .Where(o => o.Review != null)
            .Select(o => o.Review!.Rating)
            .ToList();

        AverageRating = ratings.Any() ? Math.Round(ratings.Average(), 1) : 4.5;

        double target = AverageRating < 3.8 ? 3.8 :
            AverageRating < 4.5 ? 4.5 : 5.0;
    }
}