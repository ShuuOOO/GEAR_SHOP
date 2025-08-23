namespace TL4_SHOP.Models.ViewModels
{
    public class DonHangDetailViewModel
    {
        public int DonHangId { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChiGiaoHang { get; set; }
        public DateTime NgayDatHang { get; set; }
        public decimal TongTien { get; set; }
        public int? TrangThai { get; set; }

        public string TrangThaiDonHangText
        {
            get
            {
                return TrangThai switch
                {
                    0 => "Chờ xác nhận",
                    1 => "Đang giao",
                    2 => "Hoàn tất",
                    3 => "Đã hủy",
                    4 => "Đã giao",
                    5 => "Đã hủy",
                    _ => "Không xác định"
                };
            }
        }

        public List<ChiTietDonHangViewModel> ChiTiet { get; set; } = new();
    }

    public class ChiTietDonHangViewModel
    {
        public int SanPhamId { get; set; }
        public string TenSanPham { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}


