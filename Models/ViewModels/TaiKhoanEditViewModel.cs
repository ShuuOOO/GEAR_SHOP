namespace TL4_SHOP.Models.ViewModels
{
    public class TaiKhoanEditViewModel
    {
        public int TaiKhoanId { get; set; }
        public string HoTen { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? MatKhau { get; set; } // có thể null khi không thay đổi
        public string LoaiTaiKhoan { get; set; } = null!;
        public string? VaiTro { get; set; }
    }
}
