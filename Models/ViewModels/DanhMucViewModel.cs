namespace TL4_SHOP.Models.ViewModels
{
    public class DanhMucViewModel
    {
        public int Id { get; set; }
        public string TenDanhMuc { get; set; }
        public int? DanhMucChaId { get; set; }
        public List<DanhMucViewModel> DanhMucCon { get; set; } = new List<DanhMucViewModel>();
        public int SoLuongSanPham { get; set; }
        public int? NhaCungCapId { get; set; }
    }
}
