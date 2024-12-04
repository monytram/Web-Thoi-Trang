using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebThoiTrang.Areas.Admin.Models;
using WebThoiTrang.Data;
using WebThoiTrang.Models.Entities;
using AutoMapper.QueryableExtensions;
using WebThoiTrang.Mappings;

namespace WebThoiTrang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index(string searchString, string sortOrder, int? pageNumber)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .AsNoTracking();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => p.Name.Contains(searchString)
                    || p.Description.Contains(searchString));
            }

            // Sắp xếp
            query = sortOrder switch
            {
                "name_desc" => query.OrderByDescending(p => p.Name),
                "price" => query.OrderBy(p => p.BasePrice),
                "price_desc" => query.OrderByDescending(p => p.BasePrice),
                "date" => query.OrderBy(p => p.CreatedAt),
                "date_desc" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Name),
            };

            int pageSize = 10;
            var products = await query
                .ToPaginatedListAsync<Product, ProductViewModel>(_mapper, pageNumber ?? 1, pageSize);


            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new ProductViewModel
            {
                AvailableCategories = await _context.Categories
                    .Where(c => c.IsActive)
                    .ProjectTo<CategoryViewModel>(_mapper.ConfigurationProvider)
                    .ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model, List<IFormFile> images)
        {
            if (ModelState.IsValid)
            {
                var product = _mapper.Map<Product>(model);
                product.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                product.CreatedAt = DateTime.UtcNow;

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Xử lý hình ảnh
                if (images != null && images.Any())
                {
                    var uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "uploads/products");
                    Directory.CreateDirectory(uploadPath);

                    foreach (var image in images)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        var filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        var productImage = new ProductImage
                        {
                            ProductId = product.Id,
                            ImageUrl = "/uploads/products/" + fileName,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.ProductImages.Add(productImage);
                    }

                    await _context.SaveChangesAsync();
                }

                SetSuccessMessage("Thêm sản phẩm thành công");
                return RedirectToAction(nameof(Index));
            }

                model.AvailableCategories = await _context.Categories
                 .Where(c => c.IsActive)
                 .ProjectTo<CategoryViewModel>(_mapper.ConfigurationProvider)
                 .ToListAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            var viewModel = _mapper.Map<ProductViewModel>(product);
            viewModel.AvailableCategories = await _context.Categories
                 .Where(c => c.IsActive)
                 .ProjectTo<CategoryViewModel>(_mapper.ConfigurationProvider)
                 .ToListAsync();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var product = await _context.Products.FindAsync(id);
                    _mapper.Map(model, product);
                    product.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    product.UpdatedAt = DateTime.UtcNow;

                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    SetSuccessMessage("Cập nhật sản phẩm thành công");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProductExists(id))
                        return NotFound();
                    throw;
                }
            }

            model.AvailableCategories = await _context.Categories
                 .Where(c => c.IsActive)
                 .ProjectTo<CategoryViewModel>(_mapper.ConfigurationProvider)
                 .ToListAsync();
            return View(model);
        }

        private async Task<bool> ProductExists(int id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }
    }
}
