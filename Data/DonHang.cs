using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class DonHang
{
    public int DonHangId { get; set; }

    public int? KhachHangId { get; set; }

    public DateTime NgayDatHang { get; set; }

    public decimal PhiVanChuyen { get; set; }

    public decimal TongTien { get; set; }

    public int? DiaChiId { get; set; }

    public int TrangThaiId { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual DiaChi? DiaChi { get; set; }

    public virtual KhachHang? KhachHang { get; set; }

    public virtual TrangThaiDonHang TrangThai { get; set; } = null!;
}
