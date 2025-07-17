using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class HangHoa
{
    public int Id { get; set; }

    public string? Ten { get; set; }

    public decimal? GiaSanPham { get; set; }

    public virtual Loai? Loai { get; set; }
}
