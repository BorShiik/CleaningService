using CleanDeal.Model.Models;

namespace CleanDeal.Repositories.IRepositories
{
    public interface ILoyaltyRepository
    {
        Task AddAsync(LoyaltyTransaction transaction);
        Task<IEnumerable<LoyaltyTransaction>> GetByUserAsync(string userId);
        Task<int> GetBalanceAsync(string userId);
    }
}
