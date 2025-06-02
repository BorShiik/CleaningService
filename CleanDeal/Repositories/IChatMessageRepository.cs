using CleanDeal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanDeal.Repositories
{
    public interface IChatMessageRepository
    {
        Task AddAsync(ChatMessage message);
        Task<List<ChatMessage>> GetMessagesByOrderIdAsync(int orderId);
    }
}