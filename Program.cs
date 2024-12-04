using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebThoiTrang.Data;
using WebThoiTrang.Data.Interfaces;
using WebThoiTrang.Data.Repositories;
using WebThoiTrang.Data.Seeding;
using WebThoiTrang.Models.Entities;
using WebThoiTrang.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Thêm DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "YourAppCookie"; // Đặt tên cho cookie
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(30); // Thời gian cookie hết hạn
    });

// Đăng ký các service
builder.Services.AddScoped<IAuthService, AuthService>();
// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Add Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

//builder.Services.AddScoped<ICategoryService, CategoryService>();
//builder.Services.AddScoped<IProductService, ProductService>();
//builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Cấu hình route cho area và route mặc định
app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Thêm seed data nếu cần
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // Đảm bảo database được tạo và cập nhật
        await DbSeeder.SeedData(context);
        // Seed Roles
        if (!context.Roles.Any())
        {
            var roles = new List<Role>
            {
                new Role 
                { 
                    Id = Guid.NewGuid().ToString(),
                    Name = "ADMIN",
                    Description = "Administrator"
                },
                new Role
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "CUSTOMER",
                    Description = "Customer"
                }
            };
            context.Roles.AddRange(roles);
            context.SaveChanges();
        }

        // Seed Admin User
        if (!context.Users.Any(u => u.Email == "admin@example.com"))
        {
            var adminRole = context.Roles.First(r => r.Name == "ADMIN");
            var hashedPassword = new AuthService(context).HashPassword("Admin@123");
            
            var adminUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin@example.com",
                Email = "admin@example.com",
                PasswordHash = hashedPassword,
                FullName = "Administrator",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            
            context.Users.Add(adminUser);
            context.SaveChanges();

            context.UserRoles.Add(new UserRole 
            { 
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            });
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();