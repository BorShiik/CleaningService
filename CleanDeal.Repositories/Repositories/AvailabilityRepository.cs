using CleanDeal.Model.Data;
using CleanDeal.Model.Models;
using CleanDeal.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CleanDeal.Repositories.Repositories;

public class AvailabilityRepository : IAvailabilityRepository
{
    private readonly ApplicationDbContext _ctx;

    public AvailabilityRepository(ApplicationDbContext ctx)
        => _ctx = ctx;

    public async Task<IList<Availability>> GetForCleanerAsync(string cleanerId)
    {
        return await _ctx.Availabilities
            .Where(a => a.CleanerId == cleanerId)
            .OrderBy(a => a.Date)
            .ThenBy(a => a.StartTime)
            .ToListAsync();       
    }

    public async Task AddAsync(Availability item)
    {
        _ctx.Availabilities.Add(item);
        await _ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _ctx.Availabilities.FindAsync(id);
        if (entity is not null)
        {
            _ctx.Availabilities.Remove(entity);
            await _ctx.SaveChangesAsync();
        }
    }
}