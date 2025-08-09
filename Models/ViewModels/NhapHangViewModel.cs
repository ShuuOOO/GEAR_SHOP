
namespace TL4_SHOP.Data
{
    public class NhapHangViewModel
    {
        public int SanPhamId { get; set; }
        public int NhaCungCapId { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGiaNhap { get; set; }

        public List<NhaCungCap>? DanhSachNhaCungCap { get; set; }
        public List<SanPham>? DanhSachSanPham { get; set; }
    }
}
