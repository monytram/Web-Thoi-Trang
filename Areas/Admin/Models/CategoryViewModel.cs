using System.ComponentModel.DataAnnotations;

namespace WebThoiTrang.Areas.Admin.Models
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(200, ErrorMessage = "Tên danh mục không được vượt quá 200 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Slug không được để trống")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Slug chỉ được chứa chữ thường, số và dấu gạch ngang")]
        [Display(Name = "Slug")]
        public string Slug { get; set; }

        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; }

        [Display(Name = "Danh mục cha")]
        public int? ParentId { get; set; }

        [Display(Name = "Thứ tự hiển thị")]
        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation property cho dropdown list
        public List<CategoryViewModel> AvailableParentCategories { get; set; }
    }
}
