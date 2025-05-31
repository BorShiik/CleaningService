using CleanDeal.Models;

namespace CleanDeal.Repositories
{
    public interface ICleaningOrderRepository
    {
        Task<CleaningOrder?> GetByIdAsync(int id);
        Task<IEnumerable<CleaningOrder>> GetAllAsync();
        Task<IEnumerable<CleaningOrder>> GetByUserIdAsync(string userId);
        Task<IEnumerable<CleaningOrder>> GetRecentOrdersAsync(int count);

        Task AddAsync(CleaningOrder order);
        Task UpdateAsync(CleaningOrder order);
        Task DeleteAsync(int id);

        Task<int> CountAsync();
    }
}
