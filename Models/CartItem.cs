namespace TL4_SHOP.Models
{
    public class CartItem
    {
        public int SanPhamId { get; set; }
        public string TenSanPham { get; set; }
        public string HinhAnh { get; set; }
        public decimal Gia { get; set; }
        public int SoLuong { get; set; }

        public decimal ThanhTien => Gia * SoLuong;
    }
}
