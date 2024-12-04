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
    public class UserController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string searchString, string role, string status, int? pageNumber)
        {
            var query = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .AsNoTracking();

            // Apply filters
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(u =>
                    u.UserName.Contains(searchString) ||
                    u.Email.Contains(searchString) ||
                    u.FullName.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.UserRoles.Any(ur => ur.Role.Id == role));
            }

            if (!string.IsNullOrEmpty(status))
            {
                var isActive = status == "true";
                query = query.Where(u => u.IsActive == isActive);
            }

            // Get roles for dropdown
            ViewBag.Roles = await _context.Roles.ToListAsync();
            ViewBag.SelectedRole = role;
            ViewBag.SelectedStatus = status;

            int pageSize = 10;
            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .ToPaginatedListAsync<User, UserViewModel>(_mapper, pageNumber ?? 1, pageSize);

            return View(users);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View(new UserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại trong hệ thống");
                    ViewBag.Roles = await _context.Roles.ToListAsync();
                    return View(model);
                }

                var user = new User
                {
                    Id = Guid.NewGuid().ToString(), // Generate new ID
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    IsActive = model.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    PasswordHash = HashPassword(model.Password) // Implement password hashing
                };

                _context.Users.Add(user);

                // Add user roles
                if (model.Roles != null && model.Roles.Any())
                {
                    foreach (var roleId in model.Roles)
                    {
                        user.UserRoles.Add(new UserRole
                        {
                            UserId = user.Id,
                            RoleId = roleId
                        });
                    }
                }

                await _context.SaveChangesAsync();
                SetSuccessMessage("Thêm người dùng thành công");
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            var viewModel = _mapper.Map<UserViewModel>(user);
            viewModel.Roles = user.UserRoles.Select(ur => ur.RoleId).ToList();

            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                    return NotFound();

                // Update user properties
                user.FullName = model.FullName;
                user.PhoneNumber = model.PhoneNumber;
                user.Address = model.Address;
                user.IsActive = model.IsActive;

                // Update roles
                user.UserRoles.Clear();
                if (model.Roles != null && model.Roles.Any())
                {
                    foreach (var roleId in model.Roles)
                    {
                        user.UserRoles.Add(new UserRole
                        {
                            UserId = user.Id,
                            RoleId = roleId
                        });
                    }
                }

                await _context.SaveChangesAsync();
                SetSuccessMessage("Cập nhật người dùng thành công");
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            SetSuccessMessage($"{(user.IsActive ? "Kích hoạt" : "Khóa")} tài khoản thành công");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string id, string newPassword)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.PasswordHash = HashPassword(newPassword); // Implement password hashing
            await _context.SaveChangesAsync();

            return Ok();
        }

        private string HashPassword(string password)
        {
            // Implement your password hashing logic here
            // Example using BCrypt:
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
