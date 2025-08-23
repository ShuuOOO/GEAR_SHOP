using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class ChiTietNhapHang
{
    public int ChiTietNhapHangId { get; set; }

    public int? PhieuNhapId { get; set; }

    public int? SanPhamId { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGiaNhap { get; set; }

    public decimal TongTien { get; set; }

    public virtual NhapHang? PhieuNhap { get; set; }

    public virtual SanPham? SanPham { get; set; }
}
