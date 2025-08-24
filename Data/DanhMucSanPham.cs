using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TL4_SHOP.Data;

public partial class DanhMucSanPham
{
    [Key]  // [THÊM]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // [THÊM]
    public int DanhMucId { get; set; }

    public string TenDanhMuc { get; set; } = null!;

    public string MoTa { get; set; } = null!;

    public int? DanhMucChaId { get; set; }

    public virtual DanhMucSanPham? DanhMucCha { get; set; }

    public virtual ICollection<DanhMucSanPham> DanhMucCon { get; set; } = new List<DanhMucSanPham>();

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}