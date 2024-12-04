using System.ComponentModel.DataAnnotations;

namespace WebThoiTrang.Areas.Admin.Models
{
    public class ProductVariantViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Kích thước không được để trống")]
        [Display(Name = "Kích thước")]
        public string Size { get; set; }

        [Required(ErrorMessage = "Màu sắc không được để trống")]
        [Display(Name = "Màu sắc")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Mã SKU không được để trống")]
        [Display(Name = "Mã SKU")]
        public string SKU { get; set; }

        [Required(ErrorMessage = "Số lượng tồn kho không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Số lượng tồn kho")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Giá không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; }
    }
}
