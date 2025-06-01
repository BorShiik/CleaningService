using CleanDeal.Data;
using CleanDeal.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanDeal.Repositories
{
    public class ChatMessageRepository: IChatMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatMessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ChatMessage?> GetByIdAsync(int id)
        {
            return await _context.ChatMessages
                          .Include(m => m.User)
                          .Include(m => m.CleaningOrder)
                          .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesForOrderAsync(int orderId)
        {
            return await _context.ChatMessages
                          .Include(m => m.User)
                          .Where(m => m.CleaningOrderId == orderId)
                          .OrderBy(m => m.SentAt)
                          .ToListAsync();
        }

        public async Task AddAsync(ChatMessage message)
        {
            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();
        }
    }
}
