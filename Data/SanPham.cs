using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class SanPham
{
    public int SanPhamId { get; set; }

    public string TenSanPham { get; set; } = null!;

    public string MoTa { get; set; } = null!;

    public decimal Gia { get; set; }

    public int SoLuongTon { get; set; }

    public string HinhAnh { get; set; } = null!;

    public int? DanhMucId { get; set; }

    public int NhaCungCapId { get; set; }

    public bool? LaNoiBat { get; set; }

    public string? ChiTiet { get; set; }

    public decimal? GiaSauGiam { get; set; }

    public string? ThongSoKyThuat { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<ChiTietNhapHang> ChiTietNhapHangs { get; set; } = new List<ChiTietNhapHang>();

    public virtual DanhMucSanPham? DanhMuc { get; set; }

    public virtual KhoHang? KhoHang { get; set; }

    public virtual NhaCungCap NhaCungCap { get; set; } = null!;

    public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
}
