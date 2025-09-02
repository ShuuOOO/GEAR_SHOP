using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class DonHang
{
    public int DonHangId { get; set; }
    public int? TaiKhoanId { get; set; }
    //public int? KhachHangId{ get; set; }

    public DateTime NgayDatHang { get; set; }

    public decimal PhiVanChuyen { get; set; }

    public decimal TongTien { get; set; }

    public int? DiaChiId { get; set; }

    public int TrangThaiId { get; set; }

    public string? DiaChiGiaoHang { get; set; }

    public string? GhiChu { get; set; }

    public string? PhuongThucThanhToan { get; set; }

    public string? SoDienThoai { get; set; }

    public string? TenKhachHang { get; set; }

    public string? TrangThaiDonHangText { get; set; }

    public string? EmailNguoiDat { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual DiaChi? DiaChi { get; set; }

    public virtual KhachHang? KhachHang { get; set; }

    public virtual TrangThaiDonHang TrangThai { get; set; } = null!;
}
