using TL4_SHOP.Data; // Dùng model trong Data

namespace TL4_SHOP.Models.ViewModels
{
    public class ShopViewModel
    {
        public List<TL4_SHOP.Data.SanPham> SanPhams { get; set; } = new();
        public List<TL4_SHOP.Data.NhaCungCap> NhaCungCaps { get; set; } = new();
        public List<TL4_SHOP.Data.DanhMucSanPham> DanhMucs { get; set; } = new();
        public List<PriceRangeViewModel> PriceRanges { get; set; } = new();

        public string? SearchTerm { get; set; }
        public int? DanhMucId { get; set; }
        public int? NhaCungCapId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; }

        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 20;
    }

    public class PriceRangeViewModel
    {
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public string Display => $"{Min:N0}đ - {Max:N0}đ";
        public int ProductCount { get; set; }
    }
}
