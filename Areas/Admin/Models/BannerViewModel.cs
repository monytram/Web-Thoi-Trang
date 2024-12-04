using System.ComponentModel.DataAnnotations;

namespace WebThoiTrang.Areas.Admin.Models
{
    public class BannerViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề banner")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string Title { get; set; }

        [Display(Name = "Hình ảnh")]
        public string ImageUrl { get; set; }

        [Display(Name = "Liên kết")]
        public string Link { get; set; }

        [Display(Name = "Thứ tự hiển thị")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Banner Settings/Positions
        [Display(Name = "Vị trí hiển thị")]
        public List<string> Positions { get; set; }
        public List<BannerSettingViewModel> BannerSettings { get; set; }

        public BannerViewModel()
        {
            Positions = new List<string>();
            BannerSettings = new List<BannerSettingViewModel>();
            IsActive = true;
        }
    }

    public class BannerSettingViewModel
    {
        public int Id { get; set; }
        public int BannerId { get; set; }
        public string Position { get; set; }
        public bool IsActive { get; set; }
    }
}
