using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class NhapHang
{
    public int PhieuNhapId { get; set; }

    public int? NhaCungCapId { get; set; }

    public DateTime NgayNhap { get; set; }

    public int? NhanVienId { get; set; }
    public int? TaiKhoanId { get; set; }

    public virtual ICollection<ChiTietNhapHang> ChiTietNhapHangs { get; set; } = new List<ChiTietNhapHang>();

    public virtual NhaCungCap? NhaCungCap { get; set; }

    public virtual NhanVien? NhanVien { get; set; }
}
