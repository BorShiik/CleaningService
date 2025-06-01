using CleanDeal.Data;
using CleanDeal.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanDeal.Repositories;

public class CleaningOrderRepository : ICleaningOrderRepository
{
    private readonly ApplicationDbContext _ctx;
    public CleaningOrderRepository(ApplicationDbContext ctx) => _ctx = ctx;

    /* --------------------------- CREATE --------------------------- */
    public async Task AddAsync(CleaningOrder order, IEnumerable<int> serviceIds)
    {
        var services = await _ctx.ServiceTypes
                                 .Where(s => serviceIds.Contains(s.Id))
                                 .ToListAsync();

        if (!services.Any())
            throw new InvalidOperationException("Brak wybranych usług.");

        foreach (var s in services)
            order.Items.Add(new OrderItem { ServiceTypeId = s.Id, Price = s.BasePrice });

        order.TotalPrice = order.Items.Sum(i => i.Price);
        order.ServiceTypeId = services.First().Id;          // główna usługa (dla listy)

        _ctx.CleaningOrders.Add(order);
        await _ctx.SaveChangesAsync();
    }

    /* --------------------------- UPDATE --------------------------- */
    public async Task UpdateAsync(CleaningOrder order, IEnumerable<int> serviceIds)
    {
        order.Items.Clear();

        var services = await _ctx.ServiceTypes
                                 .Where(s => serviceIds.Contains(s.Id))
                                 .ToListAsync();

        if (!services.Any())
            throw new InvalidOperationException("Brak wybranych usług.");

        foreach (var s in services)
            order.Items.Add(new OrderItem { ServiceTypeId = s.Id, Price = s.BasePrice });

        order.TotalPrice = order.Items.Sum(i => i.Price);
        order.ServiceTypeId = services.First().Id;

        _ctx.CleaningOrders.Update(order);
        await _ctx.SaveChangesAsync();
    }

    /* --------------------------- USUWANIE --------------------------- */
    public async Task DeleteAsync(int id)
    {
        var o = await _ctx.CleaningOrders.FindAsync(id);
        if (o is null) return;
        _ctx.CleaningOrders.Remove(o);
        await _ctx.SaveChangesAsync();
    }

    /* --------------------------- SELECTy --------------------------- */
    public Task<int> CountAsync() => _ctx.CleaningOrders.CountAsync();

    public async Task<CleaningOrder?> GetByIdAsync(int id) =>
        await _ctx.CleaningOrders
                  .Include(o => o.ServiceType)
                  .Include(o => o.Items).ThenInclude(i => i.ServiceType)
                  .Include(o => o.Payment)
                  .Include(o => o.Review)
                  .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<IEnumerable<CleaningOrder>> GetAllAsync() =>
        await _ctx.CleaningOrders
                  .Include(o => o.User)
                  .Include(o => o.ServiceType)
                  .Include(o => o.Items).ThenInclude(i => i.ServiceType)
                  .Include(o => o.Payment)
                  .Include(o => o.Review)
                  .ToListAsync();

    public async Task<IEnumerable<CleaningOrder>> GetByUserIdAsync(string userId) =>
        await _ctx.CleaningOrders
                  .Include(o => o.ServiceType)
                  .Include(o => o.Items).ThenInclude(i => i.ServiceType)
                  .Include(o => o.Payment)
                  .Include(o => o.Review)
                  .Where(o => o.UserId == userId)
                  .ToListAsync();

    public async Task<IEnumerable<CleaningOrder>> GetAvailableAsync() =>
        await _ctx.CleaningOrders
                  .Include(o => o.ServiceType)
                  .Where(o => o.Status == OrderStatus.WaitingForCleaner)
                  .ToListAsync();

    public async Task<IEnumerable<CleaningOrder>> GetByCleanerAsync(string cleanerId) =>
        await _ctx.CleaningOrders
                  .Include(o => o.ServiceType)
                  .Where(o => o.CleanerId == cleanerId)
                  .ToListAsync();

    /* ---------------------- akcje sprzątacza ---------------------- */
    public async Task AcceptAsync(int id, string cleanerId)
    {
        var o = await _ctx.CleaningOrders.FindAsync(id);
        if (o is null || o.Status != OrderStatus.WaitingForCleaner) return;
        o.Status = OrderStatus.InProcess;
        o.CleanerId = cleanerId;
        await _ctx.SaveChangesAsync();
    }

    public async Task<IEnumerable<CleaningOrder>> GetRecentOrdersAsync(int count) =>
    await _ctx.CleaningOrders
              .Include(o => o.User)
              .Include(o => o.ServiceType)
              .OrderByDescending(o => o.Date)
              .Take(count)
              .ToListAsync();

    public async Task CompleteAsync(int id, string cleanerId)
    {
        var o = await _ctx.CleaningOrders.FindAsync(id);
        if (o is null || o.CleanerId != cleanerId) return;
        o.Status = OrderStatus.Finished;
        await _ctx.SaveChangesAsync();
    }
}
