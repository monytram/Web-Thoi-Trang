using Microsoft.AspNetCore.Identity;
using WebThoiTrang.Models.Entities;

namespace WebThoiTrang.Areas.Admin.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

        public ApplicationUser()
        {
            UserRoles = new HashSet<UserRole>();
            Orders = new HashSet<Order>();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }
    }
}
