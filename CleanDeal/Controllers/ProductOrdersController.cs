using AutoMapper;
using CleanDeal.DTOs;
using CleanDeal.Models;
using CleanDeal.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanDeal.Controllers;

[Authorize]
public class ProductOrdersController : Controller
{
    private readonly IProductOrderRepository _repo;
    private readonly IMapper _mapper;

    public ProductOrdersController(IProductOrderRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<ProductOrder> orders;
        if (User.IsInRole("Admin"))
        {
            orders = await _repo.GetAllAsync();
        }
        else
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            orders = await _repo.GetByUserIdAsync(userId);
        }

        var dto = _mapper.Map<IEnumerable<ProductOrderDTO>>(orders);
        return View(dto);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _repo.GetByIdAsync(id);
        if (order == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!User.IsInRole("Admin") && order.UserId != userId)
            return Forbid();

        var dto = _mapper.Map<ProductOrderDTO>(order);
        return View(dto);
    }
}