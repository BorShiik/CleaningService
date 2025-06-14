using CleanDeal.Data;
using CleanDeal.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanDeal.Repositories
{
    public class ServiceTypeRepository : IServiceTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(ServiceType serviceType)
        {
            _context.ServiceTypes.Add(serviceType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var st = await _context.ServiceTypes.FindAsync(id);
            if (st != null)
            {
                _context.ServiceTypes.Remove(st);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ServiceType>> GetAllAsync()
        {
            return await _context.ServiceTypes.ToListAsync();
        }

        public async Task<ServiceType?> GetByIdAsync(int id)
        {
            return await _context.ServiceTypes.FindAsync(id);
        }

        public async Task UpdateAsync(ServiceType serviceType)
        {
            _context.ServiceTypes.Update(serviceType);
            await _context.SaveChangesAsync();
        }
    }
}
