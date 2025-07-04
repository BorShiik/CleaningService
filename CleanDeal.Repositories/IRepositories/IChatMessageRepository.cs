using CleanDeal.Model.Models;

namespace CleanDeal.Repositories.IRepositories
{
    public interface IChatMessageRepository
    {
        Task AddAsync(ChatMessage message);
        Task<List<ChatMessage>> GetMessagesByOrderIdAsync(int orderId);
    }
}