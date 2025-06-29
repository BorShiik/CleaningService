using CleanDeal.Models;

namespace CleanDeal.Repositories;

public interface IAvailabilityRepository
{
    Task<IList<Availability>> GetForCleanerAsync(string cleanerId);
    Task AddAsync(Availability item);
    Task DeleteAsync(int id);
}