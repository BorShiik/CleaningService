using CleanDeal.Models;

namespace CleanDeal.Repositories;

public interface ICleaningOrderRepository
{
    Task AddAsync(CleaningOrder order, IEnumerable<int> serviceIds);
    Task UpdateAsync(CleaningOrder order, IEnumerable<int> serviceIds);
    Task DeleteAsync(int id);

    Task<CleaningOrder?> GetByIdAsync(int id);
    Task<IEnumerable<CleaningOrder>> GetAllAsync();
    Task<IEnumerable<CleaningOrder>> GetByUserIdAsync(string userId);
    Task<IEnumerable<CleaningOrder>> GetRecentOrdersAsync(int count);   // ⬅️
    Task<IEnumerable<CleaningOrder>> GetAvailableAsync();
    Task<IEnumerable<CleaningOrder>> GetByCleanerAsync(string cleanerId);
    Task<int> CountAsync();

    Task AcceptAsync(int id, string cleanerId);
    Task CompleteAsync(int id, string cleanerId);
}
