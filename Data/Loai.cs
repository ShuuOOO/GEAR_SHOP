using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class Loai
{
    public int MaLoai { get; set; }

    public string TenLoai { get; set; } = null!;

    public string TenLoaiAlias { get; set; } = null!;

    public string? MoTa { get; set; }

    public string? Hinh { get; set; }

    public int SoLuong { get; set; }

    public virtual HangHoa MaLoaiNavigation { get; set; } = null!;
}
