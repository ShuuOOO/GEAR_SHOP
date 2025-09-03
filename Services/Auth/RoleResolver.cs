using System;
using System.Collections.Generic;

namespace TL4_SHOP.Services.Auth
{
    public interface IRoleResolver
    {
        string ToAppRole(string? loaiTaiKhoan, string? vaiTroText);
        string ToDisplayName(string appRole);
    }

    public class RoleResolver : IRoleResolver
    {
        private static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
        {
            ["nhân viên quản lý sản phẩm"] = AppRoles.ProductManager,
            ["nhan vien quan ly san pham"] = AppRoles.ProductManager,
            ["nhân viên quản lý đơn hàng"] = AppRoles.OrderManager,
            ["nhan vien quan ly don hang"] = AppRoles.OrderManager,
            ["nhân viên quản lý nhân sự"] = AppRoles.HRManager,
            ["nhan vien quan ly nhan su"] = AppRoles.HRManager,
            ["nhân viên chăm sóc khách hàng"] = AppRoles.CustomerCare,
            ["nhan vien cham soc khach hang"] = AppRoles.CustomerCare,
            ["admin"] = AppRoles.Admin,
            ["khách hàng"] = "KhachHang",
            ["khach hang"] = "KhachHang",
            ["customer"] = "KhachHang"
        };

        public string ToAppRole(string? loaiTaiKhoan, string? vaiTroText)
        {
            if (string.Equals(loaiTaiKhoan, "Admin", StringComparison.OrdinalIgnoreCase))
                return AppRoles.Admin;

            var key = (vaiTroText ?? "").Trim().ToLowerInvariant();
            return Map.TryGetValue(key, out var role) ? role : "KhachHang"; // fallback
        }

        public string ToDisplayName(string appRole) => appRole switch
        {
            AppRoles.ProductManager => "Nhân viên quản lý sản phẩm",
            AppRoles.OrderManager => "Nhân viên quản lý đơn hàng",
            AppRoles.HRManager => "Nhân viên quản lý nhân sự",
            AppRoles.CustomerCare => "Nhân viên chăm sóc khách hàng",
            AppRoles.Admin => "Admin",
            _ => "Khách hàng"
        };
    }
}
