namespace GEAR_SHOP.Models.ViewModels
{
    public class BaoCaoDoanhThuVM
    {
        public string Mode { get; set; } = "day";  // day | month | year
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? Year { get; set; }

        public List<string> Labels { get; set; } = new();
        public List<decimal> Values { get; set; } = new();

        public int TongSoDonHang { get; set; }
        public int TongSoLuong { get; set; }
        public decimal TongDoanhThu { get; set; }
        public decimal TongLoiNhuan { get; set; }
    }
}
