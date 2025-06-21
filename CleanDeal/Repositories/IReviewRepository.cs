using CleanDeal.DTOs;
using CleanDeal.Models;

namespace CleanDeal.Repositories
{
    public interface IReviewRepository
    {
        Task<Review?> GetByIdAsync(int id);
        Task<Review?> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<Review>> GetAllAsync();
        Task<double> GetAverageRatingAsync();
        Task<IEnumerable<CleanerRatingDTO>> GetAverageRatingByCleanerAsync();
        Task<IEnumerable<Review>> GetRecentAsync(int count);
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(int id);
    }
}
