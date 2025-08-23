using System.ComponentModel.DataAnnotations;

namespace TL4_SHOP.Models.ViewModels
{
    public class SanPhamViewModel
    {
        public int SanPhamId { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự")]
        public string TenSanPham { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống")]
        public string MoTa { get; set; }

        [Required(ErrorMessage = "Giá không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Gia { get; set; }

        [Required(ErrorMessage = "Số lượng tồn không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn phải lớn hơn hoặc bằng 0")]
        public int SoLuongTon { get; set; }

        public string? HinhAnh { get; set; }

        [Display(Name = "Chọn ảnh sản phẩm")]
        public IFormFile? HinhAnhFile { get; set; }

        [Display(Name = "Danh mục")]
        public int? DanhMucId { get; set; } // nullable int

        [Required(ErrorMessage = "Vui lòng chọn nhà cung cấp")]
        [Display(Name = "Nhà cung cấp")]
        public int NhaCungCapId { get; set; } // required int
    }
}