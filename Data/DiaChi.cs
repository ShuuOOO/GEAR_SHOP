using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class DiaChi
{
    public int DiaChiId { get; set; }

    public int KhachHangId { get; set; }

    public string? TenNguoiNhan { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? SoNha { get; set; }

    public string? PhuongXa { get; set; }

    public string? QuanHuyen { get; set; }

    public string? ThanhPho { get; set; }

    public string? QuocGia { get; set; }

    public string? ZipCode { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual KhachHang KhachHang { get; set; } = null!;
}
