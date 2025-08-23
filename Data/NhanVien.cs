using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class NhanVien
{
    public int NhanVienId { get; set; }

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string? VaiTro { get; set; }

    public virtual ICollection<NhapHang> NhapHangs { get; set; } = new List<NhapHang>();

    public virtual ICollection<TaoTaiKhoan> TaoTaiKhoans { get; set; } = new List<TaoTaiKhoan>();
}
