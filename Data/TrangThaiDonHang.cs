using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class TrangThaiDonHang
{
    public int TrangThaiId { get; set; }

    public string TenTrangThai { get; set; } = null!;

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
