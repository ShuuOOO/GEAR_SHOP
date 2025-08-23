using System.ComponentModel.DataAnnotations;

namespace TL4_SHOP.Models
{
    public class UserAccount
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu.")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public bool RememberMe { get; set; }
    }
}
