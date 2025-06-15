using CleanDeal.Models;

namespace CleanDeal.Repositories
{
    public interface IProductOrderRepository
    {
        Task<ProductOrder?> GetByIdAsync(int id);
        Task<IEnumerable<ProductOrder>> GetByUserIdAsync(string userId);
        Task AddAsync(ProductOrder order);
        Task UpdateAsync(ProductOrder order);
    }
}