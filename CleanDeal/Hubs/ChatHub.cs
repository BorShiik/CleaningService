using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using CleanDeal.Models;
using CleanDeal.Repositories;
using Microsoft.AspNetCore.Identity;

namespace CleanDeal.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatMessageRepository _chatRepo;
        private readonly ICleaningOrderRepository _orderRepo;

        public ChatHub(IChatMessageRepository chatRepo, ICleaningOrderRepository orderRepo)
        {
            _chatRepo = chatRepo;
            _orderRepo = orderRepo;
        }

        public async Task JoinOrderGroup(int orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"order-{orderId}");
        }

        public async Task SendOrderMessage(int orderId, string content, string receiverId)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId))
                throw new Exception("SenderId is null");

            var message = new ChatMessage
            {
                CleaningOrderId = orderId,
                Content = content,
                SenderId = senderId,
                ReceiverId = string.IsNullOrWhiteSpace(receiverId) ? null : receiverId,
                SentAt = DateTime.UtcNow
            };
            await _chatRepo.AddAsync(message);

            string senderName = Context.User?.Identity?.Name ?? senderId;

            await Clients.Group($"order-{orderId}").SendAsync("ReceiveMessage", new
            {
                OrderId = orderId,
                Content = content,
                SenderId = senderId,
                SenderName = senderName,
                ReceiverId = string.IsNullOrWhiteSpace(receiverId) ? null : receiverId,
                SentAt = message.SentAt
            });
        }
    }
}