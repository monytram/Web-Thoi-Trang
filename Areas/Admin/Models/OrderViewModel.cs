using System.ComponentModel.DataAnnotations;

namespace WebThoiTrang.Areas.Admin.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Khách hàng")]
        public string UserId { get; set; }
        public string UserFullName { get; set; }

        [Display(Name = "Tổng tiền hàng")]
        public decimal SubTotal { get; set; }

        [Display(Name = "Phí vận chuyển")]
        public decimal ShippingFee { get; set; }

        [Display(Name = "Giảm giá")]
        public decimal Discount { get; set; }

        [Display(Name = "Tổng thanh toán")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Trạng thái đơn hàng")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống")]
        [Display(Name = "Địa chỉ giao hàng")]
        public string ShippingAddress { get; set; }

        [Display(Name = "Trạng thái thanh toán")]
        public string PaymentStatus { get; set; }

        [Display(Name = "Mã vận đơn")]
        public string TrackingNumber { get; set; }

        [Display(Name = "Ghi chú")]
        public string Note { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public List<OrderDetailViewModel> OrderDetails { get; set; }
    }
}
