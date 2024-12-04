using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThoiTrang.Areas.Admin.Models;
using WebThoiTrang.Data;
using WebThoiTrang.Mappings;
using WebThoiTrang.Models.Entities;

namespace WebThoiTrang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BannerController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BannerController(ApplicationDbContext context,
                              IMapper mapper,
                              IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(int? pageNumber)
        {
            var query = _context.Banners
                .Include(b => b.BannerSettings)
                .OrderBy(b => b.DisplayOrder);

            int pageSize = 10;
            var banners = await query
                .ToPaginatedListAsync<Banner, BannerViewModel>(_mapper, pageNumber ?? 1, pageSize);

            return View(banners);
        }

        public IActionResult Create()
        {
            ViewBag.BannerPositions = new List<string> { "Home_Top", "Home_Middle", "Home_Bottom", "Category_Top" };
            return View(new BannerViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BannerViewModel model, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    var fileName = await SaveImage(imageFile);
                    model.ImageUrl = fileName;
                }

                var banner = _mapper.Map<Banner>(model);
                banner.CreatedAt = DateTime.UtcNow;

                if (model.Positions != null)
                {
                    banner.BannerSettings = model.Positions.Select(p => new BannerSetting
                    {
                        Position = p,
                        IsActive = true
                    }).ToList();
                }

                _context.Banners.Add(banner);
                await _context.SaveChangesAsync();

                SetSuccessMessage("Thêm banner thành công");
                return RedirectToAction(nameof(Index));
            }

            ViewBag.BannerPositions = new List<string> { "Home_Top", "Home_Middle", "Home_Bottom", "Category_Top" };
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var banner = await _context.Banners
                .Include(b => b.BannerSettings)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (banner == null)
                return NotFound();

            var viewModel = _mapper.Map<BannerViewModel>(banner);
            ViewBag.BannerPositions = new List<string> { "Home_Top", "Home_Middle", "Home_Bottom", "Category_Top" };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BannerViewModel model, IFormFile imageFile)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var banner = await _context.Banners
                        .Include(b => b.BannerSettings)
                        .FirstOrDefaultAsync(b => b.Id == id);

                    if (banner == null)
                        return NotFound();

                    if (imageFile != null)
                    {
                        DeleteImage(banner.ImageUrl);
                        var fileName = await SaveImage(imageFile);
                        banner.ImageUrl = fileName;
                    }

                    _mapper.Map(model, banner);
                    banner.UpdatedAt = DateTime.UtcNow;

                    // Update banner settings
                    banner.BannerSettings.Clear();
                    if (model.Positions != null)
                    {
                        foreach (var position in model.Positions)
                        {
                            banner.BannerSettings.Add(new BannerSetting
                            {
                                Position = position,
                                IsActive = true
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    SetSuccessMessage("Cập nhật banner thành công");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await BannerExists(id))
                        return NotFound();
                    throw;
                }
            }

            ViewBag.BannerPositions = new List<string> { "Home_Top", "Home_Middle", "Home_Bottom", "Category_Top" };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
                return NotFound();

            DeleteImage(banner.ImageUrl);
            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();

            SetSuccessMessage("Xóa banner thành công");
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveImage(IFormFile file)
        {
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/banners");
            Directory.CreateDirectory(uploadPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/banners/" + fileName;
        }

        private void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return;

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                // Log error
                //_logger.LogError($"Error deleting file {filePath}: {ex.Message}");
            }
        }

        private async Task<bool> BannerExists(int id)
        {
            return await _context.Banners.AnyAsync(b => b.Id == id);
        }
    }
}
