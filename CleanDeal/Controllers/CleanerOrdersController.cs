using System.Security.Claims;
using AutoMapper;
using CleanDeal.DTOs;
using CleanDeal.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanDeal.Controllers;

[Authorize(Roles = "Cleaner")]
public class CleanerOrdersController : Controller
{
    private readonly ICleaningOrderRepository _repo;
    private readonly IMapper _mapper;
    private readonly string _cleanerId;

    public CleanerOrdersController(ICleaningOrderRepository repo,
                                   IMapper mapper,
                                   IHttpContextAccessor ctx)
    {
        _repo = repo;
        _mapper = mapper;
        _cleanerId = ctx.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }

    public async Task<IActionResult> Available()
    {
        var list = (await _repo.GetAvailableAsync())
                   .Select(o => _mapper.Map<CleanerAvailableOrderDTO>(o));
        return View(list);
    }

    public async Task<IActionResult> My()
    {
        var list = (await _repo.GetByCleanerAsync(_cleanerId))
                   .Select(o => _mapper.Map<CleanerMyOrderDTO>(o));
        return View(list);
    }

    [HttpPost]
    public async Task<IActionResult> Accept(int id)
    {
        await _repo.AcceptAsync(id, _cleanerId);
        return RedirectToAction(nameof(My));
    }

    [HttpPost]
    public async Task<IActionResult> Complete(int id)
    {
        await _repo.CompleteAsync(id, _cleanerId);
        return RedirectToAction(nameof(My));
    }
}
