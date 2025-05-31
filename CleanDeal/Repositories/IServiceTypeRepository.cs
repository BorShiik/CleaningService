using CleanDeal.Models;

namespace CleanDeal.Repositories
{
    public interface IServiceTypeRepository
    {
        Task<ServiceType?> GetByIdAsync(int id);
        Task<IEnumerable<ServiceType>> GetAllAsync();
        Task AddAsync(ServiceType serviceType);
        Task UpdateAsync(ServiceType serviceType);
        Task DeleteAsync(int id);
    }
}
