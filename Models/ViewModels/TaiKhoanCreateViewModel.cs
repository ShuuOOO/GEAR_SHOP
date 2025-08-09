namespace TL4_SHOP.Models.ViewModels
{
    public class TaiKhoanCreateViewModel
    {
        public string HoTen { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string MatKhau { get; set; } = null!;
        public string LoaiTaiKhoan { get; set; } = null!;
        public string VaiTro { get; set; }
    }
}
