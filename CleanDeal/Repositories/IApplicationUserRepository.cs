using CleanDeal.Models;

namespace CleanDeal.Repositories
{
    public interface IApplicationUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string userId);
        Task<ApplicationUser?> FindByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<int> CountAsync();
        Task UpdateAsync(ApplicationUser user);
        Task DeleteAsync(string userId);
    }
}
