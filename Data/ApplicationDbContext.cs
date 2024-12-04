using Microsoft.EntityFrameworkCore;
using WebThoiTrang.Models.Entities;

namespace WebThoiTrang.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<ShippingMethod> ShippingMethods { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<BannerSetting> BannerSettings { get; set; }
        public DbSet<InventoryHistory> InventoryHistory { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình khóa chính cho bảng UserRoles (junction table)
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // Cấu hình relationships
            modelBuilder.Entity<UserRole>()
                 .HasOne(ur => ur.User)
                 .WithMany(u => u.UserRoles)
                 .HasForeignKey(ur => ur.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category configuration
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.ChildCategories)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product configuration
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.CreatedByUser)
                .WithMany()
                .HasForeignKey(p => p.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.UpdatedByUser)
                .WithMany()
                .HasForeignKey(p => p.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // ProductVariant configuration
            modelBuilder.Entity<ProductVariant>()
                .HasOne(pv => pv.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProductImage configuration
            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order configuration
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.PaymentMethod)
                .WithMany()
                .HasForeignKey(o => o.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingMethod)
                .WithMany()
                .HasForeignKey(o => o.ShippingMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            // OrderDetail configuration
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.ProductVariant)
                .WithMany()
                .HasForeignKey(od => od.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            // BannerSetting configuration
            modelBuilder.Entity<BannerSetting>()
                .HasOne(bs => bs.Banner)
                .WithMany(b => b.BannerSettings)
                .HasForeignKey(bs => bs.BannerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BannerSetting>()
                .HasOne(bs => bs.Setting)
                .WithMany()
                .HasForeignKey(bs => bs.SettingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình cho InventoryHistory
            modelBuilder.Entity<InventoryHistory>()
                .HasOne(ih => ih.ProductVariant)
                .WithMany()
                .HasForeignKey(ih => ih.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryHistory>()
                .HasOne(ih => ih.User)
                .WithMany()
                .HasForeignKey(ih => ih.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryHistory>()
                .HasOne(ih => ih.CreatedByUser)
                .WithMany()
                .HasForeignKey(ih => ih.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict); 

            // PaymentMethod configuration
            modelBuilder.Entity<PaymentMethod>()
                .HasOne(pm => pm.CreatedByUser)
                .WithMany()
                .HasForeignKey(pm => pm.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentMethod>()
                .HasOne(pm => pm.UpdatedByUser)
                .WithMany()
                .HasForeignKey(pm => pm.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // ShippingMethod configuration
            modelBuilder.Entity<ShippingMethod>()
                .HasOne(sm => sm.CreatedByUser)
                .WithMany()
                .HasForeignKey(sm => sm.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShippingMethod>()
                .HasOne(sm => sm.UpdatedByUser)
                .WithMany()
                .HasForeignKey(sm => sm.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // AuditLog configuration
            modelBuilder.Entity<AuditLog>()
                .HasOne(al => al.User)
                .WithMany()
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình decimal properties
            modelBuilder.Entity<Product>()
                .Property(p => p.BasePrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.DiscountValue)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProductVariant>()
                .Property(pv => pv.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.SubTotal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.ShippingFee)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.Discount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.SubTotal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PaymentMethod>()
                .Property(pm => pm.BaseCost)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ShippingMethod>()
                .Property(sm => sm.BaseCost)
                .HasColumnType("decimal(18,2)");
        }
    }
}
