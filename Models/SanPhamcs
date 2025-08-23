using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TL4_SHOP.Data;
using TL4_SHOP.Models;

namespace TL4_SHOP.Models
{
    public class SanPham
    {
        [Key]
        public int SanPhamId { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(100)]
        public string TenSanPham { get; set; }

        [StringLength(500)]
        public string MoTa { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Gia { get; set; }

        [Display(Name = "Hình ảnh")]
        public string? HinhAnh { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int SoLuongTon { get; set; }

        [NotMapped]
        public IFormFile? HinhAnhFile { get; set; }

        public int? DanhMucId { get; set; }
        public int NhaCungCapId { get; set; }
        
        [ForeignKey("DanhMucId")]
        public virtual DanhMuc? DanhMuc { get; set; }

        [ForeignKey("NhaCungCapId")]
        public virtual NhaCungCap? NhaCungCap { get; set; }

        public bool? LaNoiBat { get; set; }

    }
}
