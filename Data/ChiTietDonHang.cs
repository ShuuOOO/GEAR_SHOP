using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class ChiTietDonHang
{
    public int ChiTietId { get; set; }

    public int DonHangId { get; set; }

    public int SanPhamId { get; set; }

    public decimal DonGia { get; set; }

    public int SoLuong { get; set; }

    public decimal ThanhTien { get; set; }

    public virtual DonHang DonHang { get; set; } = null!;

    public virtual SanPham SanPham { get; set; } = null!;
}
