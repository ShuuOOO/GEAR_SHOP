using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class KhoHang
{
    public int SanPhamId { get; set; }

    public int SoLuongTon { get; set; }

    public virtual SanPham SanPham { get; set; } = null!;
}
