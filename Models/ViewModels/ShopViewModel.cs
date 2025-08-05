using TL4_SHOP.Models;
using TL4_SHOP.Data;

namespace TL4_SHOP.Models.ViewModels
{
    public class ShopViewModel
    {
        public List<TL4_SHOP.Data.SanPham> SanPhams { get; set; }
        public List<DanhMucSanPham> DanhMucs { get; set; } = new List<DanhMucSanPham>();
        public List<NhaCungCap> NhaCungCaps { get; set; } = new List<NhaCungCap>();

        // Filter parameters
        public string? SearchTerm { get; set; }
        public int? DanhMucId { get; set; }
        public int? NhaCungCapId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; }

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        // Price ranges for filter
        public List<PriceRange> PriceRanges { get; set; } = new List<PriceRange>
        {
            new PriceRange { Min = 0, Max = 100000, Display = "0đ - 100.000đ" },
            new PriceRange { Min = 100000, Max = 200000, Display = "100.000đ - 200.000đ" },
            new PriceRange { Min = 200000, Max = 300000, Display = "200.000đ - 300.000đ" },
            new PriceRange { Min = 300000, Max = 400000, Display = "300.000đ - 400.000đ" },
            new PriceRange { Min = 400000, Max = 500000, Display = "400.000đ - 500.000đ" },
            new PriceRange { Min = 500000, Max = null, Display = "Trên 500.000đ" }
        };
    }

    public class PriceRange
    {
        public decimal Min { get; set; }
        public decimal? Max { get; set; }
        public string Display { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }
}