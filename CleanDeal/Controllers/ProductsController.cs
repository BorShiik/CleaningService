using AutoMapper;
using Azure;
using CleanDeal.DTO.DTOs;
using CleanDeal.Model.Models;
using CleanDeal.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class ProductsController : Controller
{
    private readonly IProductRepository _productRepo;
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;

    public ProductsController(IProductRepository productRepo,
                              IWebHostEnvironment env,
                              IMapper mapper)
    {
        _productRepo = productRepo;
        _env = env;
        _mapper = mapper;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        IEnumerable<Product> products = User.IsInRole("Cleaner")
                ? await _productRepo.GetByCategoryAsync(ProductCategory.Cleaner)
                : User.IsInRole("Client")
                    ? await _productRepo.GetByCategoryAsync(ProductCategory.Client)
                    : await _productRepo.GetAllAsync();

        return View(_mapper.Map<IEnumerable<ProductDTO>>(products));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new ProductDTO());

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductDTO dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var product = _mapper.Map<Product>(dto);

        if (dto.ImageFile is { Length: > 0 })
        {
            using var ms = new MemoryStream();
            await dto.ImageFile.CopyToAsync(ms);
            product.Image = ms.ToArray();
            product.ImageMimeType = dto.ImageFile.ContentType;
        }

        await _productRepo.AddAsync(product);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product == null) return NotFound();

        return View(_mapper.Map<ProductDTO>(product));
    }

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductDTO model)
    {
        if (!ModelState.IsValid) return View(model);

        var product = await _productRepo.GetByIdAsync(id);
        if (product == null) return NotFound();

        _mapper.Map(model, product);        

        if (model.ImageFile is { Length: > 0 })
        {
            using var ms = new MemoryStream();
            await model.ImageFile.CopyToAsync(ms);
            product.Image = ms.ToArray();
            product.ImageMimeType = model.ImageFile.ContentType;
        }

        await _productRepo.UpdateAsync(product);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product == null) return NotFound();
        return View(_mapper.Map<ProductDTO>(product));
    }

    [HttpPost, ActionName("Delete"), Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _productRepo.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Image(int id)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product?.Image == null) return NotFound();

        Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";

        return File(product.Image, product.ImageMimeType ?? "image/png");
    }
}
