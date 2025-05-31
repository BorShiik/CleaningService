using CleanDeal.Data;
using CleanDeal.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanDeal.Repositories
{
    public class PaymentRepository: IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Payment?> GetByIdAsync(int id)
        {
           return await _context.Payments.Include(p => p.CleaningOrder)
                         .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Payment?> GetByOrderIdAsync(int orderId)
        {
           return await _context.Payments.Include(p => p.CleaningOrder)
                          .FirstOrDefaultAsync(p => p.CleaningOrderId == orderId);
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments.Include(p => p.CleaningOrder)
                          .ToListAsync();
        }

        public async Task AddAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetTotalAmountAsync()
        {
           return await _context.Payments.SumAsync(p => p.Amount);
        }
    }
}

