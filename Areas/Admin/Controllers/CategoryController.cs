using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThoiTrang.Areas.Admin.Models;
using WebThoiTrang.Data;
using WebThoiTrang.Models.Entities;

namespace WebThoiTrang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CategoryController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.ParentCategory)
                .OrderBy(c => c.DisplayOrder)
                .ProjectTo<CategoryViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CategoryViewModel
            {
                AvailableParentCategories = await _context.Categories
                    .Where(c => c.IsActive)
                    .ProjectTo<CategoryViewModel>(_mapper.ConfigurationProvider)
                    .ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = _mapper.Map<Category>(model);
                category.CreatedAt = DateTime.UtcNow;

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                SetSuccessMessage("Thêm danh mục thành công");
                return RedirectToAction(nameof(Index));
            }

            model.AvailableParentCategories = await _context.Categories
                .Where(c => c.IsActive)
                .ProjectTo<CategoryViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return View(model);
        }
    }
}
