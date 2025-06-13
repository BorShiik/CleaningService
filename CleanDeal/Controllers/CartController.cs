using CleanDeal.Models;
using CleanDeal.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace CleanDeal.Controllers;

[Authorize(Roles = "Cleaner")]
public class CartController : Controller
{
    private readonly IProductRepository _productRepo;

    private const string CartKey = "CLEANER_CART";

    public CartController(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    [HttpPost]
    public async Task<IActionResult> Add(int id)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product == null || product.Category != ProductCategory.Cleaner)
            return NotFound();

        var cart = GetCart();
        if (cart.TryGetValue(id, out var qty))
            cart[id] = qty + 1;
        else
            cart[id] = 1;

        SaveCart(cart);
        return RedirectToAction("Index", "Products");
    }

    public async Task<IActionResult> Index()
    {
        var cart = GetCart();
        var items = new List<(Product Product, int Quantity)>();
        foreach (var kv in cart)
        {
            var prod = await _productRepo.GetByIdAsync(kv.Key);
            if (prod != null)
            {
                prod.StockQuantity--;
                items.Add((prod, kv.Value));
            }
        }
        return View(items);
    }

    [HttpPost]
    public IActionResult Clear()
    {
        HttpContext.Session.Remove(CartKey);
        return RedirectToAction(nameof(Index));
    }

    private Dictionary<int, int> GetCart()
    {
        var json = HttpContext.Session.GetString(CartKey);
        if (string.IsNullOrEmpty(json))
            return new Dictionary<int, int>();
        return JsonSerializer.Deserialize<Dictionary<int, int>>(json) ?? new Dictionary<int, int>();
    }

    private void SaveCart(Dictionary<int, int> cart)
    {
        var json = JsonSerializer.Serialize(cart);
        HttpContext.Session.SetString(CartKey, json);
    }
}