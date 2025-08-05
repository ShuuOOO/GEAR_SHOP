using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TL4_SHOP.Models
{
    public class DanhMuc
    {
        public int DanhMucId { get; set; }

        [Required]
        public string TenDanhMuc { get; set; }

        // Navigation: 1 DanhMuc có nhiều SanPham
        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}
