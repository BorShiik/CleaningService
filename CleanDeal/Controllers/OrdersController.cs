using AutoMapper;
using CleanDeal.DTOs;
using CleanDeal.Models;
using CleanDeal.Repositories;
using CleanDeal.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace CleanDeal.Controllers;

[Authorize(Roles = "Client,Admin")]
public class OrdersController : Controller
{
    private readonly ICleaningOrderRepository _orders;
    private readonly IServiceTypeRepository _services;
    private readonly IMapper _mapper;

    public OrdersController(ICleaningOrderRepository orders,
                            IServiceTypeRepository services,
                            IMapper mapper)
    {
        _orders = orders;
        _services = services;
        _mapper = mapper;
    }

    /* ------------------------------------------------  LISTA  ------------------------------------------------ */

    public async Task<IActionResult> Index()
    {
        IEnumerable<CleaningOrder> src = User.IsInRole("Admin")
            ? await _orders.GetAllAsync()
            : await _orders.GetByUserIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        return View(_mapper.Map<IEnumerable<CleaningOrderDTO>>(src));
    }

    /* ---------------------------------------------  CREATE / EDIT  --------------------------------------------- */

    private async Task FillServiceOptionsAsync(OrderCreateViewModel vm)
    {
        var list = (await _services.GetAllAsync()).ToList();
        vm.ServiceTypeOptions = list.Select(s => new SelectListItem
        {
            Value = s.Id.ToString(),
            Text = $"{s.Name} ({s.BasePrice:c})"
        });
        vm.PriceMap = list.ToDictionary(s => s.Id, s => s.BasePrice);
    }

    public async Task<IActionResult> Create()
    {
        var vm = new OrderCreateViewModel();
        await FillServiceOptionsAsync(vm);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderCreateViewModel vm)
    {
        if (!vm.SelectedServiceTypeIds.Any())
            ModelState.AddModelError(string.Empty, "Wybierz przynajmniej jedną usługę.");

        if (!ModelState.IsValid)
        {
            await FillServiceOptionsAsync(vm);
            return View(vm);
        }

        var order = _mapper.Map<CleaningOrder>(vm);
        order.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        order.Status = OrderStatus.WaitingForCleaner;

        await _orders.AddAsync(order, vm.SelectedServiceTypeIds);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var order = await _orders.GetByIdAsync(id);
        if (order is null || order.Status == OrderStatus.Finished) return NotFound();

        var vm = _mapper.Map<OrderCreateViewModel>(order);
        await FillServiceOptionsAsync(vm);
        return View("Create", vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, OrderCreateViewModel vm)
    {
        if (id != vm.Id) return NotFound();

        if (!vm.SelectedServiceTypeIds.Any())
            ModelState.AddModelError(string.Empty, "Wybierz przynajmniej jedną usługę.");

        if (!ModelState.IsValid)
        {
            await FillServiceOptionsAsync(vm);
            return View("Create", vm);
        }

        var order = await _orders.GetByIdAsync(id);
        if (order is null || order.Status == OrderStatus.Finished) return NotFound();

        order.Date = vm.Date;
        order.Address = vm.Address;

        await _orders.UpdateAsync(order, vm.SelectedServiceTypeIds);
        return RedirectToAction(nameof(Index));
    }

    /* ------------------------------------------------  DELETE  ------------------------------------------------ */

    public async Task<IActionResult> Delete(int id)
    {
        var order = await _orders.GetByIdAsync(id);
        if (order is null) return NotFound();

        if (!User.IsInRole("Admin") && order.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            return Forbid();

        if (order.Status == OrderStatus.Finished || order.Payment != null)
        {
            TempData["Error"] = "Nie można usunąć ukończonego lub opłaconego zlecenia.";
            return RedirectToAction(nameof(Index));
        }

        return View(_mapper.Map<CleaningOrderDTO>(order));
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var order = await _orders.GetByIdAsync(id);
        if (order is null) return NotFound();

        if (!User.IsInRole("Admin") && order.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            return Forbid();

        if (order.Status == OrderStatus.Finished || order.Payment != null)
        {
            TempData["Error"] = "Nie można usunąć ukończonego lub opłaconego zlecenia.";
            return RedirectToAction(nameof(Index));
        }

        await _orders.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
