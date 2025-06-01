using AutoMapper;
using CleanDeal.DTOs;
using CleanDeal.Models;
using CleanDeal.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanDeal.Controllers
{
    [Authorize]
    public class ChatController: Controller
    {
        private readonly ICleaningOrderRepository _orderRepo;
        private readonly IChatMessageRepository _chatRepo;
        private readonly IMapper _mapper;

        public ChatController(ICleaningOrderRepository orderRepo, IChatMessageRepository chatRepo, IMapper mapper)
        {
            _orderRepo = orderRepo;
            _chatRepo = chatRepo;
            _mapper = mapper;
        }
        public async Task<IActionResult> Order(int? id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _orderRepo.GetByUserIdAsync(userId);
            var orderListDtos = _mapper.Map<List<OrderListDTO>>(orders);
            var receiverId = "";
            if (id.HasValue)
            {
                var selectedOrder = orders.FirstOrDefault(o => o.Id == id.Value);
                if (selectedOrder != null)
                {
                    if (selectedOrder.UserId == userId)
                        receiverId = selectedOrder.AssignedCleanerId; 
                    else
                        receiverId = selectedOrder.UserId;
                }
            }


            List<ChatMessageDTO>? messages = null;
            if (id.HasValue)
            {
                var chatMsgs = await _chatRepo.GetMessagesForOrderAsync(id.Value);
                messages = _mapper.Map<List<ChatMessageDTO>>(chatMsgs);
            }

            var dto = new ChatPageViewModel
            {
                Orders = orderListDtos,
                SelectedOrderId = id,
                Messages = messages,
                ReceiverId = receiverId
            };
            ViewBag.CurrentUserId = userId;
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(int orderId)
        {
            var msgs = await _chatRepo.GetMessagesForOrderAsync(orderId);
            var dto = _mapper.Map<IEnumerable<ChatMessageDTO>>(msgs);
            return Json(dto);
        }
            
        [HttpPost]
        public async Task<IActionResult> PostMessage(int orderId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return BadRequest("Treść wiadomości pusta.");

            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            string? receiverId = null;
            if (order.UserId == userId) 
            {
                receiverId = order.AssignedCleanerId; 
            }
            else if (User.IsInRole("Cleaner"))
            {
                receiverId = order.UserId;
            }
            else if (User.IsInRole("Admin"))
            {
                receiverId = order.UserId;
            }

            var message = new ChatMessage
            {
                CleaningOrderId = orderId,
                Content = content,
                SenderId = userId,
                ReceiverId = string.IsNullOrWhiteSpace(receiverId) ? null : receiverId,
                SentAt = DateTime.UtcNow
            };
            await _chatRepo.AddAsync(message);
            return Ok();                               
        }
    }
}
