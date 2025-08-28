using Microsoft.AspNetCore.Mvc;

namespace TL4_SHOP.Models.ViewModels
{
    public class NhapHangItemViewModel
    {
        public int? SanPhamId { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGiaNhap { get; set; }
    }

    public class NhapHangCreateViewModel
    {
        public int? NhaCungCapId { get; set; }
        public DateTime? NgayNhap { get; set; } = DateTime.Now;
        public int? NhanVienId { get; set; } // Nếu sau này map nhân viên đăng nhập
        public int? TaiKhoanId { get; set; } // Nếu sau này map nhân viên đăng nhập

        public List<NhapHangItemViewModel> Items { get; set; } = new()
        {
            new NhapHangItemViewModel { SoLuong = 1, DonGiaNhap = 0 }
        };
    }
}

