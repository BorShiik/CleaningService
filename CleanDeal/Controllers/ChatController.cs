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
        public async Task<IActionResult> Order(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isCleaner = User.IsInRole("Cleaner") && order.UserId != userId;
            if (!User.IsInRole("Admin") && order.UserId != userId && !isCleaner)
                return Forbid();

            var messages = await _chatRepo.GetMessagesForOrderAsync(id);
            var dto = _mapper.Map<IEnumerable<ChatMessageDTO>>(messages);

            ViewBag.OrderId = id;
            ViewBag.ServiceName = order.ServiceType.Name;

            return View(dto);                         // Views/Chat/Order.cshtml
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
            if (!User.IsInRole("Admin") && order.UserId != userId)
                return Forbid();

            var message = new ChatMessage
            {
                CleaningOrderId = orderId,
                Content = content,
                UserId = userId,
                SentAt = DateTime.UtcNow
            };
            await _chatRepo.AddAsync(message);
            return Ok();                               
        }
    }
}
