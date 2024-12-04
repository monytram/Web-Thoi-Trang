using WebThoiTrang.Models.Entities;

namespace WebThoiTrang.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public List<Order> RecentOrders { get; set; }
        public List<TopSellingProductViewModel> TopSellingProducts { get; set; }
    }

    public class TopSellingProductViewModel
    {
        public string ProductName { get; set; }
        public int TotalQuantity { get; set; }
    }
}