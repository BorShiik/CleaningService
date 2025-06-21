using CleanDeal.Models;

namespace CleanDeal.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<int> CountAsync();
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
