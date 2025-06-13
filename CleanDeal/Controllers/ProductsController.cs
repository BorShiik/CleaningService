using AutoMapper;
using CleanDeal.DTOs;
using CleanDeal.Models;
using CleanDeal.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanDeal.Controllers
{
    [Authorize]
    public class ProductsController: Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepo, IMapper mapper)
        {
            _productRepo = productRepo;
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDTO dto)
        {
            if (!ModelState.IsValid) return View(dto);
            
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductDTO dto)
        {
            if (id != dto.Id) return BadRequest();
            if (!ModelState.IsValid) return View(dto);

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
    }
}
