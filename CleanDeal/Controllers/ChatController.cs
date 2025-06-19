using CleanDeal.DTOs;
using CleanDeal.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using CleanDeal.Models;
using CleanDeal.ViewModel;
using Microsoft.AspNetCore.Identity;

namespace CleanDeal.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ICleaningOrderRepository _orderRepo;
        private readonly IChatMessageRepository _chatRepo;
        private readonly UserManager<ApplicationUser> _userMgr;
        private readonly IMapper _mapper;

        public ChatController(ICleaningOrderRepository orderRepo, IChatMessageRepository chatRepo, UserManager<ApplicationUser> userMgr, IMapper mapper)
        {
            _orderRepo = orderRepo;
            _chatRepo = chatRepo;
            _userMgr = userMgr;
            _mapper = mapper;
        }

        public async Task<IActionResult> Order(int ?id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IEnumerable<CleaningOrder> orders =
                User.IsInRole("Admin")
                          ? await _orderRepo.GetAllAsync()              
                          : User.IsInRole("Cleaner")
                              ? await _orderRepo.GetByCleanerAsync(userId)
                              : await _orderRepo.GetByUserIdAsync(userId);
            var currentUser = await _userMgr.GetUserAsync(User);
            int selectedOrderId =
                (id.HasValue && orders.Any(o => o.Id == id))     
                           ? id.Value
                           : 0;
            var messages = selectedOrderId == 0
                ? new List<ChatMessage>()
                : await _chatRepo.GetMessagesByOrderIdAsync(selectedOrderId);

            if (selectedOrderId > 0 && !messages.Any())
            { 
                var admin = await _userMgr.GetUsersInRoleAsync("Admin");
                var adminUser = admin.FirstOrDefault();          
                if (adminUser != null)
                { 
                    await _chatRepo.AddAsync(new ChatMessage
                    {
                        CleaningOrderId = selectedOrderId,
                        SenderId = adminUser.Id,
                        Sender = adminUser,
                        Content = "Hello, your cleaner is on the way!",
                        SentAt = DateTime.UtcNow
                    });
                   
                    messages = await _chatRepo.GetMessagesByOrderIdAsync(selectedOrderId);
                }
            }
            var adminUsers = await _userMgr.GetUsersInRoleAsync("Admin"); 
            var adminIds = adminUsers.Select(u => u.Id).ToHashSet();
            var vm = new ChatPageViewModel
            {
                Orders = orders.ToList(),
                SelectedOrderId = selectedOrderId,
                Messages = _mapper.Map<List<ChatMessageDTO>>(messages)
                    .Select(dto => { dto.IsAdmin = adminIds.Contains(dto.SenderId); return dto; })
                    .ToList(),
                ReceiverId = null,
                Sender = currentUser
            };

            ViewBag.CurrentUserId = userId;
            ViewBag.CurrentOrderId = selectedOrderId.ToString();
            return View(vm);
        }


    }
}