using System.ComponentModel.DataAnnotations;
namespace TL4_SHOP.Models.ViewModels
{
    public class TaiKhoanEditViewModel
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string HoTen { get; set; } = null!;
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^\d{9,15}$", ErrorMessage = "Số điện thoại không hợp lệ (9–15 chữ số)")]
        public string Phone { get; set; } = null!;
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        public string? MatKhau { get; set; } // có thể null khi không thay đổi
        [Required(ErrorMessage = "Loại tài khoản không được để trống")]
        public string LoaiTaiKhoan { get; set; } = null!;
        [Required(ErrorMessage = "Vai trò không được để trống")]
        public string VaiTro { get; set; } = null!;
    }
}
