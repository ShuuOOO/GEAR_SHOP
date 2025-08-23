using System.ComponentModel.DataAnnotations;

namespace TL4_SHOP.Models.ViewModels
{
    public class ProductListItemVM
    {
        public int SanPhamID { get; set; }
        public string? TenSanPham { get; set; }
        public decimal Gia { get; set; }
        public decimal? GiaSauGiam { get; set; }
        public int SoLuongTon { get; set; }
        public string? HinhAnh { get; set; }
        public string? TenDanhMuc { get; set; }
        public string? TenNhaCungCap { get; set; }
        public bool LaNoiBat { get; set; }
    }

    public class ProductFilterVM
    {
        public string? q { get; set; }
        public int? DanhMucID { get; set; }
        public int? NhaCungCapID { get; set; }
        public bool? LaNoiBat { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ProductFormVM
    {
        public int? SanPhamID { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Tên sản phẩm")]
        public string TenSanPham { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Mô tả ngắn")]
        public string MoTa { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Giá")]
        public decimal Gia { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Số lượng tồn")]
        public int SoLuongTon { get; set; }

        public string? HinhAnh { get; set; }   // lưu đường dẫn ảnh

        [Display(Name = "Danh mục")]
        public int? DanhMucID { get; set; }

        [Required]
        [Display(Name = "Nhà cung cấp")]
        public int NhaCungCapID { get; set; }

        [Display(Name = "Nổi bật")]
        public bool LaNoiBat { get; set; }     // đảm bảo là bool (không nullable)

        [Display(Name = "Chi tiết (HTML)")]
        public string? ChiTiet { get; set; }

        [Display(Name = "Giá sau giảm")]
        public decimal? GiaSauGiam { get; set; }

        [Display(Name = "Thông số kỹ thuật (HTML)")]
        public string? ThongSoKyThuat { get; set; }
    }
}