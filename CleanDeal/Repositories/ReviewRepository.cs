using CleanDeal.Data;
using CleanDeal.DTOs;
using CleanDeal.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanDeal.Repositories
{
    public class ReviewRepository: IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _context.Reviews
                          .Include(r => r.CleaningOrder)
                          .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Review?> GetByOrderIdAsync(int orderId)
        {
            return await _context.Reviews
                          .Include(r => r.CleaningOrder)
                          .FirstOrDefaultAsync(r => r.CleaningOrderId == orderId);
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await _context.Reviews
                          .Include(r => r.CleaningOrder)
                          .ThenInclude(o => o.User)
                          .ToListAsync();
        }

        public async Task AddAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<double> GetAverageRatingAsync()
        {
            if (!await _context.Reviews.AnyAsync()) return 0;
            return await _context.Reviews.AverageAsync(r => r.Rating);
        }

        public async Task<IEnumerable<CleanerRatingDTO>> GetAverageRatingByCleanerAsync()
        {
            return await _context.Reviews
                .Include(r => r.CleaningOrder)
                .ThenInclude(o => o.Cleaner)
                .Where(r => r.CleaningOrder.CleanerId != null)
                .GroupBy(r => new { r.CleaningOrder.CleanerId, r.CleaningOrder.Cleaner!.FullName })
                .Select(g => new CleanerRatingDTO
                {
                    CleanerId = g.Key.CleanerId!,
                    CleanerName = g.Key.FullName,
                    AverageRating = g.Average(r => r.Rating)
                })
                .ToListAsync();
        }
    }
}
