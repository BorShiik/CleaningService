using CleanDeal.Models;

namespace CleanDeal.Repositories
{
    public interface IServicePackageRepository
    {
        Task<ServicePackage?> GetByIdAsync(int id);
        Task<IEnumerable<ServicePackage>> GetAllAsync();
        Task AddAsync(ServicePackage package);
        Task UpdateAsync(ServicePackage package);
        Task DeleteAsync(int id);
    }
}