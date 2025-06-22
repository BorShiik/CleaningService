using CleanDeal.Models;

namespace CleanDeal.Repositories
{
    public interface ILoyaltyRepository
    {
        Task AddAsync(LoyaltyTransaction transaction);
        Task<IEnumerable<LoyaltyTransaction>> GetByUserAsync(string userId);
        Task<int> GetBalanceAsync(string userId);
    }
}
