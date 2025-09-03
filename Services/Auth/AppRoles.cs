using Microsoft.AspNetCore.Mvc;

namespace TL4_SHOP.Services.Auth
{
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string ProductManager = "ProductManager";   // Nhân viên quản lý sản phẩm
        public const string OrderManager = "OrderManager";     // Nhân viên quản lý đơn hàng
        public const string HRManager = "HRManager";        // Nhân viên quản lý nhân sự
        public const string CustomerCare = "CustomerCare";     // Nhân viên chăm sóc khách hàng
        public static readonly string[] AllStaff = { Admin, ProductManager, OrderManager, HRManager, CustomerCare };
    }
}
