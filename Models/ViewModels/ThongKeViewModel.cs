using TL4_SHOP.Data;

namespace TL4_SHOP.Models.ViewModels
{
    public class ThongKeViewModel
    {
        public List<DoanhThuTheoNgay> DoanhThuNgay { get; set; }
        public List<DoanhThuTheoThang> DoanhThuThang { get; set; }
        public List<DoanhThuTheoNam> DoanhThuNam { get; set; }
    }
}
