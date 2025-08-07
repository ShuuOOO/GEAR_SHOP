using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class KhachHang
{
    public int KhachHangId { get; set; }

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public virtual ICollection<DiaChi> DiaChis { get; set; } = new List<DiaChi>();

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual TaoTaiKhoan? TaiKhoan { get; set; }
}
