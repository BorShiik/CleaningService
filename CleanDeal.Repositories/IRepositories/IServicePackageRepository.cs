using CleanDeal.Model.Models;

namespace CleanDeal.Repositories.IRepositories
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