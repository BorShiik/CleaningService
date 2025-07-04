using CleanDeal.Model.Data;
using CleanDeal.Model.Models;
using CleanDeal.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CleanDeal.Repositories.Repositories
{
    public class CleaningOrderRepository : ICleaningOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILoyaltyRepository _loyalty;
        public CleaningOrderRepository(ApplicationDbContext context, ILoyaltyRepository loyalty)
        {
            _context = context;
            _loyalty = loyalty;
        }
        public async Task AddAsync(CleaningOrder order)
        {
            _context.CleaningOrders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.CleaningOrders.CountAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _context.CleaningOrders.FindAsync(id);
            if (order != null)
            {
                _context.CleaningOrders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CleaningOrder>> GetAllAsync()
        {
            return await _context.CleaningOrders
             .Include(o => o.User)
             .Include(o => o.ServiceType)
             .Include(o => o.ServiceItems).ThenInclude(si => si.ServiceType)
             .Include(o => o.Payment)
             .Include(o => o.Review)
             .Include(o => o.Cleaner)
             .ToListAsync();
        }

        public async Task<CleaningOrder?> GetByIdAsync(int id)
        {
            return await _context.CleaningOrders
                .Include(o => o.User)
                .Include(o => o.ServiceType)
                .Include(o => o.ServiceItems).ThenInclude(si => si.ServiceType)
                .Include(o => o.Payment)
                .Include(o => o.Review)
                .Include(o => o.Cleaner)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<CleaningOrder>> GetByUserIdAsync(string userId)
        {
            return await _context.CleaningOrders
                .Include(o => o.ServiceType)
                .Include(o => o.ServiceItems).ThenInclude(si => si.ServiceType)
                .Include(o => o.Payment)
                .Include(o => o.Review)
                .Include(o => o.Cleaner)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CleaningOrder>> GetRecentOrdersAsync(int count)
        {
            return await _context.CleaningOrders
                .Include(o => o.User).Include(o => o.ServiceType)
                .OrderByDescending(o => o.Date)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<CleaningOrder>> GetOrdersPagedAsync(int skip, int take)
        {
            return await _context.CleaningOrders
                .Include(o => o.User)
                .Include(o => o.ServiceType)
                .OrderByDescending(o => o.Date)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task UpdateAsync(CleaningOrder order)
        {
            _context.CleaningOrders.Update(order);
            await _context.SaveChangesAsync();
        }
  
        public async Task<IEnumerable<CleaningOrder>> GetAvailableAsync() =>
        await _context.CleaningOrders
            .Include(o => o.ServiceType)
            .Include(o => o.ServiceItems).ThenInclude(si => si.ServiceType)
            .Where(o => o.Status == OrderStatus.WaitingForCleaner)
            .ToListAsync();

        public async Task<IEnumerable<CleaningOrder>> GetByCleanerAsync(string cleanerId) =>
            await _context.CleaningOrders
                .Include(o => o.ServiceType)
                .Include(o => o.ServiceItems).ThenInclude(si => si.ServiceType)
                .Where(o => o.CleanerId == cleanerId)
                .ToListAsync();

        public async Task<IEnumerable<CleaningOrder>> GetCompletedByCleanerAsync(string cleanerId) =>
            await _context.CleaningOrders
                .Include(o => o.ServiceType)
                .Include(o => o.Review)
                .Include(o => o.ServiceItems).ThenInclude(si => si.ServiceType)
                .Include(o => o.Payment)
                .Where(o => o.CleanerId == cleanerId && o.Status == OrderStatus.Finished)
                .ToListAsync();

        public async Task AcceptAsync(int id, string cleanerId)
        {
            var o = await _context.CleaningOrders.FindAsync(id);
            if (o is null || o.Status != OrderStatus.WaitingForCleaner) return;
            o.Status = OrderStatus.InProcess;
            o.CleanerId = cleanerId;
            var cleaner = await _context.Users.FindAsync(cleanerId);
            o.Cleaner = cleaner;    
            await _context.SaveChangesAsync();
        }

        public async Task CompleteAsync(int id, string cleanerId)
        {
            var o = await _context.CleaningOrders
                .Include(ord => ord.Payment)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (o is null || o.CleanerId != cleanerId) return;
            o.Status = OrderStatus.Finished;
            o.IsCompleted = true;
            await _context.SaveChangesAsync();

            if (o.Payment != null)
            {
                int points = (int)(o.Payment.Amount / 10);
                if (points > 0)
                {
                    await _loyalty.AddAsync(new LoyaltyTransaction
                    {
                        UserId = o.UserId,
                        Points = points,
                        Description = $"Order #{o.Id} completed"
                    });
                }

                var avgRating = await _context.Reviews
                    .Include(r => r.CleaningOrder)
                    .Where(r => r.CleaningOrder.CleanerId == cleanerId)
                    .AverageAsync(r => (double?)r.Rating) ?? 0;

                if (avgRating >= 4.8)
                {
                    int bonus = (int)(o.Payment.Amount * 0.05m);
                    await _loyalty.AddAsync(new LoyaltyTransaction
                    {
                        UserId = cleanerId,
                        Points = bonus,
                        Description = $"Bonus za ocenę {avgRating:F1}"
                    });
                }

                var completed = await _context.CleaningOrders
                    .CountAsync(ord => ord.CleanerId == cleanerId && ord.Status == OrderStatus.Finished);

                if (completed % 20 == 0)
                {
                    await _loyalty.AddAsync(new LoyaltyTransaction
                    {
                        UserId = cleanerId,
                        Points = 50,
                        Description = "Bonus za 20 ukończonych zleceń"
                    });
                }
            }
        }
    }
}
