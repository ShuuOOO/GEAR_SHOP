using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class DanhMucSanPham
{
    public int DanhMucId { get; set; }

    public string TenDanhMuc { get; set; } = null!;

    public string MoTa { get; set; } = null!;

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
