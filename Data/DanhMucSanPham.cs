using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class DanhMucSanPham
{
    public int DanhMucId { get; set; }

    public string TenDanhMuc { get; set; } = null!;

    public string MoTa { get; set; } = null!;

    public int? DanhMucChaId { get; set; }  // mới thêm: để xác định danh mục cha

    // --- Quan hệ với sản phẩm
    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();

    // --- Quan hệ cha - con danh mục
    public virtual DanhMucSanPham? DanhMucCha { get; set; }  // navigation đến cha
    public virtual ICollection<DanhMucSanPham> DanhMucCon { get; set; } = new List<DanhMucSanPham>(); // danh sách con
}
