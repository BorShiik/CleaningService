using CleanDeal.Data;
using CleanDeal.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanDeal.Repositories
{
    public class ApplicationUserRepository: IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string userId)
        {
            return await _context.Users
                          .Include(u => u.CleaningOrders)
                          .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<ApplicationUser?> FindByEmailAsync(string email)
        {
            return await _context.Users
                          .Include(u => u.CleaningOrders)
                          .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _context.Users
                          .Include(u => u.CleaningOrders)
                          .ToListAsync();
        }

        public async Task<int> CountAsync() =>
            await _context.Users.CountAsync();

        public async Task UpdateAsync(ApplicationUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
