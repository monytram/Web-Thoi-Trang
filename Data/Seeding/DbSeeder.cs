using WebThoiTrang.Models.Entities;

namespace WebThoiTrang.Data.Seeding
{
    public static class DbSeeder
    {
        public static async Task SeedData(ApplicationDbContext context)
        {
            try
            {
                if (!context.Roles.Any())
                {
                    var roles = new List<Role>
                    {
                        new Role
                        {
                            Id = "1",
                            Name = "ADMIN",
                            Description = "Administrator",
                        },
                        new Role
                        {
                            Id = "2",
                            Name = "CUSTOMER",
                            Description = "Customer",
                        }
                    };

                    await context.Roles.AddRangeAsync(roles);
                    await context.SaveChangesAsync();

                    var adminUser = new User
                    {
                        Id = "1",
                        UserName = "admin@example.com",
                        Email = "admin@example.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                        FullName = "Administrator",
                        PhoneNumber = "0123456789", // Thêm số điện thoại
                        Address = "Admin Address", // Thêm địa chỉ
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        UserRoles = new List<UserRole>() // Khởi tạo collection
                    };

                    await context.Users.AddAsync(adminUser);
                    await context.SaveChangesAsync();

                    var userRole = new UserRole
                    {
                        UserId = adminUser.Id,
                        RoleId = roles[0].Id // Admin role
                    };

                    await context.UserRoles.AddAsync(userRole);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error seeding data: " + ex.Message);
            }
        }
    }
}