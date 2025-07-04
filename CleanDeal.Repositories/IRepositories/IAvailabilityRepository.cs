using CleanDeal.Model.Models;

namespace CleanDeal.Repositories.IRepositories;

public interface IAvailabilityRepository
{
    Task<IList<Availability>> GetForCleanerAsync(string cleanerId);
    Task AddAsync(Availability item);
    Task DeleteAsync(int id);
}