namespace TL4_SHOP.Data
{
    public partial class WishlistItem
    {
        public int WishlistItemId { get; set; }
        public int WishlistId { get; set; }
        public int SanPhamId { get; set; }

        public virtual Wishlist Wishlist { get; set; } = null!;
        public virtual SanPham SanPham { get; set; } = null!;
    }
}
