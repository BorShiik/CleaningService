using CleanDeal.Models;

namespace CleanDeal.Services.Loyalty
{
    public interface ILoyaltyService
    {
        Task AwardPointsAsync(string userId, int points, string description);
        Task<int> GetBalanceAsync(string userId);
        Task<IEnumerable<LoyaltyTransaction>> GetHistoryAsync(string userId);
    }
}
