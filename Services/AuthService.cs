using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using WebThoiTrang.Data;
using WebThoiTrang.Models;
using WebThoiTrang.Models.Entities;

namespace WebThoiTrang.Services
{
    public interface IAuthService
    {
        Task<bool> ValidateUserAsync(string email, string password);
        Task<User> RegisterUserAsync(RegisterViewModel model);
        string HashPassword(string password);
        Task<User> GetUserByEmailAsync(string email);
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ValidateUserAsync(string email, string password)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

            if (user == null)
                return false;

            // Sử dụng BCrypt.Verify để kiểm tra password
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public async Task<User> RegisterUserAsync(RegisterViewModel model)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = model.Email,
                UserName = model.Email,
                PasswordHash = HashPassword(model.Password),
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Thêm role mặc định cho user mới (ví dụ: CUSTOMER)
            var customerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "CUSTOMER");
            if (customerRole != null)
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = customerRole.Id
                });
                await _context.SaveChangesAsync();
            }

            return user;
        }

        public string HashPassword(string password)
        {
            // Sử dụng BCrypt để hash password
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
