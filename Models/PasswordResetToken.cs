using System.ComponentModel.DataAnnotations;
using TL4_SHOP.Data;

namespace TL4_SHOP.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public int TaiKhoanId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public virtual TaoTaiKhoan TaiKhoan { get; set; }
    }
}