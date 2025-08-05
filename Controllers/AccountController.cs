using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;
using TL4_SHOP.Models;

namespace TL4_SHOP.Controllers
{
    public class AccountController : Controller
    {
        private readonly _4tlShopContext _context;

        public AccountController(_4tlShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserAccount account)
        {
            var user = _context.TaoTaiKhoans
                .FirstOrDefault(u => u.HoTen == account.Username && u.MatKhau == account.Password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.HoTen),
                    new Claim("TaiKhoanId", user.TaiKhoanId.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = account.RememberMe,
                        ExpiresUtc = DateTime.UtcNow.AddDays(account.RememberMe ? 7 : 1)
                    });

                TempData["Message"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Message = "Tên đăng nhập hoặc mật khẩu không đúng.";
            return View(account);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserAccount account)
        {
            if (ModelState.IsValid)
            {
                var exists = _context.TaoTaiKhoans.Any(u => u.Email == account.Username);
                if (exists)
                {
                    ViewBag.Message = "Email đã được sử dụng.";
                    return View(account);
                }

                var newUser = new TaoTaiKhoan
                {
                    HoTen = account.Username,
                    Email = account.Email,
                    Phone = account.Phone,
                    MatKhau = account.Password,
                    LoaiTaiKhoan = "KhachHang"
                };

                _context.TaoTaiKhoans.Add(newUser);
                _context.SaveChanges();

                TempData["Message"] = "Đăng ký thành công!";
                return RedirectToAction("Login");
            }

            return View(account);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear(); // xoá toàn bộ thông tin phiên
            TempData["Message"] = "Đã đăng xuất thành công!";
            return RedirectToAction("Index", "Home"); // CHUYỂN VỀ TRANG CHỦ
        }

        [HttpGet]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var username = result.Principal?.Identity?.Name;

            if (username != null)
            {
                HttpContext.Session.SetString("Username", username);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}