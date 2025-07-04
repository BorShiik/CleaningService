using CleanDeal.Model.Models;

namespace CleanDeal.Repositories.IRepositories
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
