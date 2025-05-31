using CleanDeal.Models;

namespace CleanDeal.Repositories
{
    public interface IChatMessageRepository
    {
        Task<ChatMessage?> GetByIdAsync(int id);
        Task<IEnumerable<ChatMessage>> GetMessagesForOrderAsync(int orderId);
        Task AddAsync(ChatMessage message);
    }
}
