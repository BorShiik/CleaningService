using CleanDeal.Model.Data;
using CleanDeal.Model.Models;
using CleanDeal.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CleanDeal.Repositories.Repositories
{
    public class ProductOrderRepository : IProductOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProductOrder order)
        {
            _context.ProductOrders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task<ProductOrder?> GetByIdAsync(int id)
        {
            return await _context.ProductOrders
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<ProductOrder>> GetAllAsync()
        {
            return await _context.ProductOrders
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Include(o => o.Payment)
                .ToListAsync();
        }


        public async Task<IEnumerable<ProductOrder>> GetByUserIdAsync(string userId)
        {
            return await _context.ProductOrders
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Include(o => o.Payment)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task UpdateAsync(ProductOrder order)
        {
            _context.ProductOrders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}