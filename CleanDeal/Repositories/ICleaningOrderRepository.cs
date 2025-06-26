using CleanDeal.Models;

namespace CleanDeal.Repositories
{
    public interface ICleaningOrderRepository
    {
        Task<CleaningOrder?> GetByIdAsync(int id);
        Task<IEnumerable<CleaningOrder>> GetAllAsync();
        Task<IEnumerable<CleaningOrder>> GetByUserIdAsync(string userId);
        Task<IEnumerable<CleaningOrder>> GetRecentOrdersAsync(int count);
        Task<IEnumerable<CleaningOrder>> GetOrdersPagedAsync(int skip, int take);
        Task<IEnumerable<CleaningOrder>> GetAvailableAsync();
        Task<IEnumerable<CleaningOrder>> GetByCleanerAsync(string cleanerId);
        Task<IEnumerable<CleaningOrder>> GetCompletedByCleanerAsync(string cleanerId);
        Task AcceptAsync(int id, string cleanerId);
        Task CompleteAsync(int id, string cleanerId);

        Task AddAsync(CleaningOrder order);
        Task UpdateAsync(CleaningOrder order);
        Task DeleteAsync(int id);

        Task<int> CountAsync();
    }
}
