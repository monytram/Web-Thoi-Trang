namespace WebThoiTrang.Models.Entities
{
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }

    public class Role
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }

    public class UserRole
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public bool IsActive { get; set; }
        public int? ParentId { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual Category ParentCategory { get; set; }
        public virtual ICollection<Category> ChildCategories { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public decimal BasePrice { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual Category Category { get; set; }
        public virtual User CreatedByUser { get; set; }
        public virtual User UpdatedByUser { get; set; }
        public virtual ICollection<ProductVariant> Variants { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PaymentMethodId { get; set; }
        public int ShippingMethodId { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }
        public string PaymentStatus { get; set; }
        public string TrackingNumber { get; set; }
        public string Note { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual ShippingMethod ShippingMethod { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    public class Banner
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Link { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<BannerSetting> BannerSettings { get; set; }
    }

    public class BannerSetting
    {
        public int Id { get; set; }
        public int BannerId { get; set; }
        public int SettingId { get; set; }
        public string Position { get; set; }
        public bool IsActive { get; set; }

        public virtual Banner Banner { get; set; }
        public virtual Setting Setting { get; set; }
    }

    public class Setting
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ProductVariant
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string SKU { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Product Product { get; set; }
    }

    public class ProductImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsMain { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Product Product { get; set; }
    }

    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool RequireShipping { get; set; }
        public bool IsOnlinePayment { get; set; }
        public bool IsActive { get; set; }
        public decimal BaseCost { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User CreatedByUser { get; set; }
        public virtual User UpdatedByUser { get; set; }
    }

    public class ShippingMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BaseCost { get; set; }
        public bool IsCOD { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User CreatedByUser { get; set; }
        public virtual User UpdatedByUser { get; set; }
    }

    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }

        public virtual Order Order { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
    }

    public class InventoryHistory
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public string UserId { get; set; }
        public int Quantity { get; set; }
        public string Type { get; set; }
        public string Note { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ProductVariant ProductVariant { get; set; }
        public virtual User User { get; set; }
        public virtual User CreatedByUser { get; set; }
    }

    public class AuditLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string IPAddress { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual User User { get; set; }
    }
}
