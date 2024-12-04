using System.ComponentModel.DataAnnotations;

namespace WebThoiTrang.Areas.Admin.Models
{
    public class ProductImageViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        [Required(ErrorMessage = "URL hình ảnh không được để trống")]
        [Display(Name = "URL hình ảnh")]
        public string ImageUrl { get; set; }

        [Display(Name = "Thứ tự hiển thị")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Ảnh chính")]
        public bool IsMain { get; set; }
    }
}
