using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using CleanDeal.Models;
using CleanDeal.Repositories;
using System.Threading.Tasks;
using System;

namespace CleanDeal.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatMessageRepository _chatRepo;
        private readonly ICleaningOrderRepository _orderRepo;
        private readonly IApplicationUserRepository _userRepo;

        public ChatHub(IChatMessageRepository chatRepo, ICleaningOrderRepository orderRepo, IApplicationUserRepository userRepo)
        {
            _chatRepo = chatRepo;
            _orderRepo = orderRepo;
            _userRepo = userRepo;
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
            var sender = await _userRepo.GetByIdAsync(senderId);
            if(sender==null)
                throw new Exception("Sender is null");
            var receiver = await _userRepo.GetByIdAsync(receiverId);
            bool isAdmin = Context.User.IsInRole("Admin");
            var message = new ChatMessage
            {
                CleaningOrderId = orderId,
                Content = content,
                SenderId = senderId,
                Sender = sender,
                ReceiverId = string.IsNullOrWhiteSpace(receiverId) ? null : receiverId,
                Receiver = receiver,
                SentAt = DateTime.UtcNow
            };
            await _chatRepo.AddAsync(message);

            await Clients.Group($"order-{orderId}").SendAsync("ReceiveMessage", new
            {
                OrderId = orderId,
                Content = content,
                SenderId = senderId,
                SenderName = sender.FullName,
                isAdmin = isAdmin,
                ReceiverId = message.ReceiverId,
                SentAt = message.SentAt
            });
        }
    }
}