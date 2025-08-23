using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class DoanhThuTheoThang
{
    public int Nam { get; set; }

    public int Thang { get; set; }

    public int? TongSoDonHang { get; set; }

    public int? TongSoLuong { get; set; }

    public decimal? TongDoanhThu { get; set; }

    public decimal? LoiNhuan { get; set; }
}
