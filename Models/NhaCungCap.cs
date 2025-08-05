using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TL4_SHOP.Models
{
    public class NhaCungCap
    {
        public int NhaCungCapId { get; set; }

        [Required]
        public string TenNhaCungCap { get; set; }

        public string DiaChi { get; set; }

        // Navigation: 1 NhaCungCap có nhiều SanPham
        public virtual ICollection<SanPham> SanPhams { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
