using AutoMapper;
using CleanDeal.DTOs;
using CleanDeal.Models;
using CleanDeal.Repositories;
using CleanDeal.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace CleanDeal.Controllers
{
    [Authorize(Roles = "Client,Admin")]
    public class OrdersController: Controller
    {
        private readonly ICleaningOrderRepository _orderRepo;
        private readonly IServiceTypeRepository _serviceTypeRepo;
        private readonly IApplicationUserRepository _userRepo;
        private readonly IMapper _mapper;

        public OrdersController(ICleaningOrderRepository orderRepo, IServiceTypeRepository serviceTypeRepo, IApplicationUserRepository userRepo, IMapper mapper)
        {
            _orderRepo = orderRepo;
            _serviceTypeRepo = serviceTypeRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<CleaningOrder> orders;
            if (User.IsInRole("Admin"))
            {
                orders = await _orderRepo.GetAllAsync();
            }
            else
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                orders = await _orderRepo.GetByUserIdAsync(userId);
            }

            var orderDtos = _mapper.Map<IEnumerable<CleaningOrderDTO>>(orders);
            return View(orderDtos);
        }

        public async Task<IActionResult> Create()
        {
            var serviceTypes = await _serviceTypeRepo.GetAllAsync();
            var model = new OrderCreateViewModel
            {
                Date = System.DateTime.Today.AddDays(1), 
                ServiceTypeOptions = serviceTypes.Select(st => new SelectListItem
                {
                    Value = st.Id.ToString(),
                    Text = st.Name
                })
            };

            ViewBag.ServicePrices = serviceTypes
            .ToDictionary(st => st.Id, st => st.BasePrice);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var serviceTypes = await _serviceTypeRepo.GetAllAsync();
                model.ServiceTypeOptions = serviceTypes.Select(st => new SelectListItem
                {
                    Value = st.Id.ToString(),
                    Text = st.Name
                });

                ViewBag.ServicePrices = serviceTypes
                .ToDictionary(st => st.Id, st => st.BasePrice);

                return View(model);
            }

            var newOrder = _mapper.Map<CleaningOrder>(model);
            newOrder.ServiceItems = model.ServiceTypeIds
                .Select(id => new CleaningOrderService { ServiceTypeId = id })
                .ToList();
            if (model.ServiceTypeIds.Any())
                newOrder.ServiceTypeId = model.ServiceTypeIds.First();
            string userId;
            if (User.IsInRole("Admin"))
            {
                if (string.IsNullOrWhiteSpace(model.UserEmail))
                {
                    ModelState.AddModelError("UserEmail", "Email klienta jest wymagany.");
                    var serviceTypes = await _serviceTypeRepo.GetAllAsync();
                    model.ServiceTypeOptions = serviceTypes.Select(st => new SelectListItem
                    {
                        Value = st.Id.ToString(),
                        Text = st.Name
                    });

                    ViewBag.ServicePrices = serviceTypes
                    .ToDictionary(st => st.Id, st => st.BasePrice);

                    return View(model);
                }

                var user = await _userRepo.FindByEmailAsync(model.UserEmail);
                if (user == null)
                {
                    ModelState.AddModelError("UserEmail", "Nie znaleziono użytkownika o podanym email.");
                    var serviceTypes = await _serviceTypeRepo.GetAllAsync();
                    model.ServiceTypeOptions = serviceTypes.Select(st => new SelectListItem
                    {
                        Value = st.Id.ToString(),
                        Text = st.Name
                    });

                    ViewBag.ServicePrices = serviceTypes
                    .ToDictionary(st => st.Id, st => st.BasePrice);

                    return View(model);
                }
                userId = user.Id;
            }
            else
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            newOrder.UserId = userId;
            newOrder.Status = OrderStatus.WaitingForCleaner;
            newOrder.IsCompleted = false;

            await _orderRepo.AddAsync(newOrder);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            if (!User.IsInRole("Admin") && order.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            var orderDto = _mapper.Map<CleaningOrderDTO>(order);
            return View(orderDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return NotFound();

            if (!User.IsInRole("Admin") &&
                order.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid();

            if (order.Status == OrderStatus.Finished)
            {
                TempData["Error"] = "Nie można edytować zakończonego zlecenia.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var vm = _mapper.Map<OrderCreateViewModel>(order);

            var serviceTypes = await _serviceTypeRepo.GetAllAsync();
            vm.ServiceTypeOptions = serviceTypes.Select(st => new SelectListItem
            {
                Value = st.Id.ToString(),
                Text = st.Name
            });

            ViewBag.ServicePrices = serviceTypes
            .ToDictionary(st => st.Id, st => st.BasePrice);

            return View(vm);      
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var serviceTypes = await _serviceTypeRepo.GetAllAsync();
                model.ServiceTypeOptions = serviceTypes.Select(st => new SelectListItem
                {
                    Value = st.Id.ToString(),
                    Text = st.Name
                });

                ViewBag.ServicePrices = serviceTypes
                .ToDictionary(st => st.Id, st => st.BasePrice);

                return View(model);
            }

            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return NotFound();

            if (!User.IsInRole("Admin") &&
                order.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid();

            if (order.Status == OrderStatus.Finished)
            {
                TempData["Error"] = "Nie można edytować zakończonego zlecenia.";
                return RedirectToAction(nameof(Details), new { id });
            }

            order.Date = model.Date;
            order.Address = model.Address;
            order.ServiceItems.Clear();
            foreach (var sid in model.ServiceTypeIds)
            {
                order.ServiceItems.Add(new CleaningOrderService { ServiceTypeId = sid });
            }
            if (model.ServiceTypeIds.Any())
                order.ServiceTypeId = model.ServiceTypeIds.First();

            await _orderRepo.UpdateAsync(order);
            TempData["Message"] = "Zamówienie zaktualizowano.";

            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return NotFound();

            if (!User.IsInRole("Admin") &&
                order.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid();

            if (order.Status == OrderStatus.Finished || order.Payment != null)
            {
                TempData["Error"] = "Nie można usunąć zakończonego lub opłaconego zlecenia.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var dto = _mapper.Map<CleaningOrderDTO>(order);
            return View(dto);          // Views/Orders/Delete.cshtml 
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return NotFound();

            if (!User.IsInRole("Admin") &&
                order.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid();

            if (order.Status == OrderStatus.Finished || order.Payment != null)
            {
                TempData["Error"] = "Nie można usunąć zakończonego lub opłaconego zlecenia.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await _orderRepo.DeleteAsync(id);
            TempData["Message"] = "Zamówienie usunięto.";
            return RedirectToAction(nameof(Index));
        }
    }
}
