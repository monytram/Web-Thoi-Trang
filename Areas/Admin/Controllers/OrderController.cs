using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebThoiTrang.Areas.Admin.Models;
using WebThoiTrang.Data;
using WebThoiTrang.Mappings;
using WebThoiTrang.Models.Entities;

namespace WebThoiTrang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OrderController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string searchString, string status, int? pageNumber)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(o =>
                    o.Id.ToString().Contains(searchString) ||
                    o.User.FullName.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
            }

            int pageSize = 10;
            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .ToPaginatedListAsync<Order, OrderViewModel>(_mapper, pageNumber ?? 1, pageSize);

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            var viewModel = _mapper.Map<OrderViewModel>(order);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;
            order.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            await _context.SaveChangesAsync();
            SetSuccessMessage("Cập nhật trạng thái đơn hàng thành công");

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
