using CleanDeal.Model.Data;
using CleanDeal.Model.Models;
using CleanDeal.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CleanDeal.Repositories.Repositories
{
    public class UserDiscountRepository : IUserDiscountRepository
    {
        private readonly ApplicationDbContext _context;
        public UserDiscountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserServiceDiscount discount)
        {
            _context.UserServiceDiscounts.Add(discount);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserServiceDiscount>> GetActiveByUserAsync(string userId)
        {
            return await _context.UserServiceDiscounts
                .Include(d => d.ServiceType)
                .Where(d => d.UserId == userId && !d.Redeemed)
                .ToListAsync();
        }
    }
}