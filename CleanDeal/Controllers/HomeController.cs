using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CleanDeal.Models;
using CleanDeal.Repositories;
using CleanDeal.ViewModel;

namespace CleanDeal.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IServiceTypeRepository _serviceRepo;

    public HomeController(ILogger<HomeController> logger, IServiceTypeRepository serviceRepo)
    {
        _logger = logger;
        _serviceRepo = serviceRepo;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated == true && User.IsInRole("Cleaner"))
        {
            // przekierowanie do stron y sprz¹tacza
            return RedirectToAction("Index", "CleanerOrders");
        }

        var services = await _serviceRepo.GetAllAsync();
        return View(services);
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
