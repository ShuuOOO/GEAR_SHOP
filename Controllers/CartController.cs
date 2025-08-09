using Microsoft.AspNetCore.Mvc;
using TL4_SHOP.Data;
using TL4_SHOP.Models;
using TL4_SHOP.Extensions;


namespace TL4_SHOP.Controllers
{
    public class CartController : BaseController
    {
        private readonly _4tlShopContext _context;
        private const string CART_KEY = "GioHang";

        public CartController(_4tlShopContext context) : base(context)
        {
            _context = context;
        }

        // Lấy giỏ hàng từ Session
        private List<CartItem> GetCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CART_KEY);
            if (cart == null)
            {
                cart = new List<CartItem>();
                HttpContext.Session.SetObjectAsJson(CART_KEY, cart);
            }
            return cart;
        }

        // Hiển thị giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCart();
            return View("ShoppingCart", cart);  // ← dùng đúng tên file .cshtml bạn đã tạo
        }

        // Thêm sản phẩm vào giỏ
        public IActionResult AddToCart(int id)
        {
            var product = _context.SanPhams.FirstOrDefault(p => p.SanPhamId == id);
            if (product == null) return NotFound();

            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.SanPhamId == id);

            if (item == null)
            {
                cart.Add(new CartItem
                {
                    SanPhamId = product.SanPhamId,
                    TenSanPham = product.TenSanPham,
                    Gia = product.Gia,
                    SoLuong = 1,
                    HinhAnh = product.HinhAnh ?? ""
                });
            }
            else
            {
                item.SoLuong++;
            }

            HttpContext.Session.SetObjectAsJson(CART_KEY, cart);
            return RedirectToAction("Index");
        }

        // Xóa 1 sản phẩm khỏi giỏ
        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.SanPhamId == id);
            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.SetObjectAsJson(CART_KEY, cart);
            }
            return RedirectToAction("Index");
        }

        // Cập nhật số lượng
        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.SanPhamId == id);
            if (item != null)
            {
                item.SoLuong = quantity;
                HttpContext.Session.SetObjectAsJson(CART_KEY, cart);
            }
            return RedirectToAction("Index", "Cart");
        }
        [HttpPost]
        [Route("Cart/AddToCartAjax")]
        public JsonResult AddToCart(int productId, int quantity = 1)
        {
            var product = _context.SanPhams.FirstOrDefault(p => p.SanPhamId == productId);
            if (product == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });

            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.SanPhamId == productId);

            if (item == null)
            {
                cart.Add(new CartItem
                {
                    SanPhamId = product.SanPhamId,
                    TenSanPham = product.TenSanPham,
                    Gia = product.Gia,
                    SoLuong = quantity,
                    HinhAnh = product.HinhAnh ?? ""
                });
            }
            else
            {
                item.SoLuong += quantity;
            }

            HttpContext.Session.SetObjectAsJson("GioHang", cart);

            return Json(new { success = true, cartCount = cart.Sum(i => i.SoLuong) });
        }


        [HttpGet]
        public IActionResult CartCount()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("GioHang");
            int count = cart?.Sum(x => x.SoLuong) ?? 0;
            return Json(new { count });
        }

        public IActionResult MiniCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("GioHang") ?? new();
            return PartialView("_MiniCart", cart);
        }
    }
}
