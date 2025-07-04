using CleanDeal.Model.Models;

namespace CleanDeal.Repositories.IRepositories
{
    public interface IProductOrderRepository
    {
        Task<ProductOrder?> GetByIdAsync(int id);
        Task<IEnumerable<ProductOrder>> GetAllAsync();
        Task<IEnumerable<ProductOrder>> GetByUserIdAsync(string userId);
        Task AddAsync(ProductOrder order);
        Task UpdateAsync(ProductOrder order);
    }
}