using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class PasswordResetToken
{
    public int Id { get; set; }

    public int TaiKhoanId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }

    public bool? IsUsed { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual TaoTaiKhoan TaiKhoan { get; set; } = null!;
}
