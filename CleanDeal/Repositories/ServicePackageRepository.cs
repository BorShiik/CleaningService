using CleanDeal.Data;
using CleanDeal.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanDeal.Repositories
{
    public class ServicePackageRepository : IServicePackageRepository
    {
        private readonly ApplicationDbContext _context;
        public ServicePackageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ServicePackage package)
        {
            _context.ServicePackages.Add(package);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var pkg = await _context.ServicePackages.FindAsync(id);
            if (pkg != null)
            {
                _context.ServicePackages.Remove(pkg);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ServicePackage>> GetAllAsync()
        {
            return await _context.ServicePackages
                .Include(p => p.Items)
                .ThenInclude(i => i.ServiceType)
                .ToListAsync();
        }

        public async Task<ServicePackage?> GetByIdAsync(int id)
        {
            return await _context.ServicePackages
                .Include(p => p.Items)
                .ThenInclude(i => i.ServiceType)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(ServicePackage package)
        {
            _context.ServicePackages.Update(package);
            await _context.SaveChangesAsync();
        }
    }
}