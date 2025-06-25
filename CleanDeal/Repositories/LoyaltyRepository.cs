using AspNetCoreGeneratedDocument;
using CleanDeal.Data;
using CleanDeal.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanDeal.Repositories
{
    public class LoyaltyRepository : ILoyaltyRepository
    {
        private readonly ApplicationDbContext _context;
        public LoyaltyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(LoyaltyTransaction transaction)
        {
            _context.LoyaltyTransactions.Add(transaction);
            var user = await _context.Users.FindAsync(transaction.UserId);
            if(user != null)
            {
                user.LoyaltyPoints += transaction.Points;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<LoyaltyTransaction>> GetByUserAsync(string userId)
        {
            return await _context.LoyaltyTransactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetBalanceAsync(string UserId)
        {
            var user = await _context.Users.FindAsync(UserId);
            return user?.LoyaltyPoints ?? 0;
        }
    }
}
