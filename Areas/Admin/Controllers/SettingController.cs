using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThoiTrang.Areas.Admin.Models;
using WebThoiTrang.Data;
using WebThoiTrang.Models.Entities;

namespace WebThoiTrang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;

        public SettingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _context.Settings.ToListAsync();
            var viewModel = new SettingsViewModel
            {
                Settings = settings.ToDictionary(s => s.Key, s => s.Value)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Dictionary<string, string> settings)
        {
            if (settings != null)
            {
                foreach (var setting in settings)
                {
                    if (!string.IsNullOrEmpty(setting.Key))
                    {
                        var existingSetting = await _context.Settings
                            .FirstOrDefaultAsync(s => s.Key == setting.Key);

                        if (existingSetting != null)
                        {
                            existingSetting.Value = setting.Value ?? string.Empty;
                            existingSetting.UpdatedAt = DateTime.UtcNow;
                        }
                        else
                        {
                            _context.Settings.Add(new Setting
                            {
                                Key = setting.Key,
                                Value = setting.Value ?? string.Empty,
                                Type = "string",
                                UpdatedAt = DateTime.UtcNow
                            });
                        }
                    }
                }

                await _context.SaveChangesAsync();
                SetSuccessMessage("Cập nhật cài đặt thành công");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
