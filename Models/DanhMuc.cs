using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TL4_SHOP.Models
{
    public class DanhMuc
    {
        public int DanhMucId { get; set; }

        [Required]
        public string TenDanhMuc { get; set; }

        // Khoá ngoại trỏ tới chính danh mục cha
        public int? DanhMucChaId { get; set; }

        // Navigation tới danh mục cha
        public virtual DanhMuc DanhMucCha { get; set; }

        // Navigation tới danh mục con
        public virtual ICollection<DanhMuc> DanhMucCon { get; set; }

        // Navigation: 1 DanhMuc có nhiều SanPham
        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}
