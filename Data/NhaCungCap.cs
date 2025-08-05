using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class NhaCungCap
{
    public int NhaCungCapId { get; set; }

    public string TenNhaCungCap { get; set; } = null!;

    public string DiaChi { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<NhapHang> NhapHangs { get; set; } = new List<NhapHang>();

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
