namespace TL4_SHOP.Models.ViewModels
{
    public class ProfileEditViewModel
    {
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // Đổi mật khẩu
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
