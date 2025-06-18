using CleanDeal.Data;
using CleanDeal.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanDeal.Repositories
{
    public class CleaningOrderRepository : ICleaningOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public CleaningOrderRepository(ApplicationDbContext context)
        {
            _context = context;
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
                .Include(o => o.Payment)
                .Include(o => o.Review)
                .Include(o => o.Cleaner)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<CleaningOrder>> GetByUserIdAsync(string userId)
        {
            return await _context.CleaningOrders
                .Include(o => o.ServiceType)
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

        public async Task UpdateAsync(CleaningOrder order)
        {
            _context.CleaningOrders.Update(order);
            await _context.SaveChangesAsync();
        }
  
        public async Task<IEnumerable<CleaningOrder>> GetAvailableAsync() =>
        await _context.CleaningOrders
            .Include(o => o.ServiceType)
            .Where(o => o.Status == OrderStatus.WaitingForCleaner)
            .ToListAsync();

        public async Task<IEnumerable<CleaningOrder>> GetByCleanerAsync(string cleanerId) =>
            await _context.CleaningOrders
                .Include(o => o.ServiceType)
                .Where(o => o.CleanerId == cleanerId)
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
            var o = await _context.CleaningOrders.FindAsync(id);
            if (o is null || o.CleanerId != cleanerId) return;
            o.Status = OrderStatus.Finished;
            o.IsCompleted = true;
            await _context.SaveChangesAsync();
        }
    }
}
