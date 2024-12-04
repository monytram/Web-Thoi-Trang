using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThoiTrang.Areas.Admin.Models;
using WebThoiTrang.Data;

namespace WebThoiTrang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]

    public class DashboardController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardViewModel = new DashboardViewModel
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),
                RecentOrders = await _context.Orders
                    .Include(o => o.User)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToListAsync(),
                TopSellingProducts = await _context.OrderDetails
                    .Include(od => od.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                    .GroupBy(od => od.ProductVariant.ProductId)
                    .Select(g => new TopSellingProductViewModel
                    {
                        ProductName = g.First().ProductVariant.Product.Name,
                        TotalQuantity = g.Sum(od => od.Quantity)
                    })
                    .OrderByDescending(p => p.TotalQuantity)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboardViewModel);
        }
    }
}