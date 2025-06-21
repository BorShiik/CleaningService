using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CleanDeal.Models;
using CleanDeal.Repositories;
using CleanDeal.ViewModel;
using AutoMapper;
using CleanDeal.DTOs;

namespace CleanDeal.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IServiceTypeRepository _serviceRepo;
    private readonly IReviewRepository _reviewRepo;
    private readonly IMapper _mapper;

    public HomeController(ILogger<HomeController> logger, IServiceTypeRepository serviceRepo, IReviewRepository reviewRepo, IMapper mapper)
    {
        _logger = logger;
        _serviceRepo = serviceRepo;
        _reviewRepo = reviewRepo;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated == true && User.IsInRole("Cleaner"))
        {
            return RedirectToAction("Index", "CleanerOrders");
        }

        var services = await _serviceRepo.GetAllAsync();
        var reviews = await _reviewRepo.GetRecentAsync(5);
        var reviewDtos = _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
        var vm = new HomeIndexViewModel { Services = services, Reviews = reviewDtos };
        return View(vm);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
