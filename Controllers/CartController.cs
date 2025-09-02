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

        // Lưu giỏ hàng vào Session
        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetObjectAsJson(CART_KEY, cart);
        }

        // Hiển thị giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCart();
            return View("ShoppingCart", cart);
        }

        // Thêm sản phẩm vào giỏ (redirect về Index)
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

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // Thêm sản phẩm vào giỏ bằng Ajax
        [HttpPost]
        public JsonResult AddToCartAjax(int productId, int quantity = 1)
        {
            // 🔑 Kiểm tra đăng nhập
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new
                {
                    success = false,
                    requireLogin = true,
                    message = "Bạn cần đăng nhập để thêm sản phẩm vào giỏ hàng."
                });
            }

            var product = _context.SanPhams.FirstOrDefault(p => p.SanPhamId == productId);
            if (product == null)
                return Json(new { success = false, message = "❌ Sản phẩm không tồn tại." });

            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.SanPhamId == productId);

            string message;

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

                message = $"✅ Đã thêm \"{product.TenSanPham}\" vào giỏ hàng.";
            }
            else
            {
                item.SoLuong += quantity;
                message = $"🔄 Đã cập nhật số lượng \"{product.TenSanPham}\" trong giỏ hàng.";
            }

            SaveCart(cart);

            return Json(new
            {
                success = true,
                message = message,
                cartCount = cart.Sum(i => i.SoLuong)
            });
        }


        // Xóa 1 sản phẩm khỏi giỏ
        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.SanPhamId == id);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }

        // Cập nhật số lượng
        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.SanPhamId == id);
            if (item != null && quantity > 0)
            {
                item.SoLuong = quantity;
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }

        // Trả về số lượng sản phẩm trong giỏ
        [HttpGet]
        public IActionResult CartCount()
        {
            var cart = GetCart();
            int count = cart.Sum(x => x.SoLuong);
            return Json(new { count });
        }

        // Mini Cart (hiển thị nhỏ ở header)
        public IActionResult MiniCart()
        {
            var cart = GetCart();
            return PartialView("_MiniCart", cart);
        }
    }
}
