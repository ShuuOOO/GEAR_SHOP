namespace GEAR_SHOP.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TongSanPham { get; set; }
        public int TongDonHang { get; set; }
        public int TongKhachHang { get; set; }

        public decimal DoanhThuThang { get; set; }

        // Phân rã trạng thái đơn (theo TrangThaiID)
        public int DonChoXacNhan { get; set; }   // 1
        public int DonDaXacNhan { get; set; }    // 2
        public int DonDangGiao { get; set; }     // 3
        public int DonGiaoThanhCong { get; set; }// 4
        public int DonDaHuy { get; set; }        // 5

        // Biểu đồ 7 ngày gần nhất
        public List<string> NgayLabels { get; set; } = new();
        public List<decimal> DoanhThuNgay { get; set; } = new();
    }
}