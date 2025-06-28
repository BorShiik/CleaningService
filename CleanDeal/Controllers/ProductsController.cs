using AutoMapper;
using CleanDeal.DTOs;
using CleanDeal.Models;
using CleanDeal.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CleanDeal.Controllers
{
    [Authorize]
    public class ProductsController: Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepo, IPaymentRepository paymentRepo,
            IWebHostEnvironment env, IMapper mapper)
        {
            _productRepo = productRepo;
            _paymentRepo = paymentRepo;
            _env = env;
            _mapper = mapper;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {

            IEnumerable<Product> products;
            if (User.IsInRole("Cleaner"))
            {
                products = await _productRepo.GetByCategoryAsync(ProductCategory.Cleaner);
            }
            else if (User.IsInRole("Client"))
            {
                products = await _productRepo.GetByCategoryAsync(ProductCategory.Client);
            }
            else
            {
                products = await _productRepo.GetAllAsync();
            }

            var dto = _mapper.Map<IEnumerable<ProductDTO>>(products);
            return View(dto);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDTO dto, IFormFile? image)
        {
            if (!ModelState.IsValid) return View(dto);

            dto.ImageUrl = await SaveImageAsync(image);  

            var product = _mapper.Map<Product>(dto);
            await _productRepo.AddAsync(product);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound(); 
            var dto =_mapper.Map<ProductDTO>(product);
            return View(dto);
        }

        [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductDTO dto, IFormFile? image)
        {
            if (id != dto.Id) return BadRequest();
            if (!ModelState.IsValid) return View(dto);

            if (image is { Length: > 0 })
            {
                var old = await _productRepo.GetByIdAsync(id);
                if (!string.IsNullOrEmpty(old?.ImageUrl))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, old.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                dto.ImageUrl = await SaveImageAsync(image);
            }

            var product = _mapper.Map<Product>(dto);
            await _productRepo.UpdateAsync(product);

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound();
            var dto = _mapper.Map<ProductDTO>(product);
            return View(dto);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<string?> SaveImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            var folder = Path.Combine(_env.WebRootPath, "uploads", "products");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(folder, fileName);

            await using var fs = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(fs);

            return "/uploads/products/" + fileName;
        }


    }
}
