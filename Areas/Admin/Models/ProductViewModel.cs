using System.ComponentModel.DataAnnotations;

namespace WebThoiTrang.Areas.Admin.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Slug không được để trống")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Slug chỉ được chứa chữ thường, số và dấu gạch ngang")]
        [Display(Name = "Slug")]
        public string Slug { get; set; }

        [Required(ErrorMessage = "Danh mục không được để trống")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Giá gốc không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá gốc phải lớn hơn 0")]
        [Display(Name = "Giá gốc")]
        public decimal BasePrice { get; set; }

        [Display(Name = "Loại giảm giá")]
        public string DiscountType { get; set; }

        [Display(Name = "Giá trị giảm giá")]
        public decimal? DiscountValue { get; set; }

        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties cho dropdown lists
        public List<CategoryViewModel> AvailableCategories { get; set; }
        public List<ProductVariantViewModel> Variants { get; set; }
        public List<ProductImageViewModel> ProductImages { get; set; }
    }
}
