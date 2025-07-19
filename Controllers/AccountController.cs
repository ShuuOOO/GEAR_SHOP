using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TL4_SHOP.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace TL4_SHOP.Controllers
{
    public class AccountController : Controller
    {
        private const string validUsername = "admin";
        private const string validPassword = "123";

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserAccount account)
        {
            if (account.Username == validUsername && account.Password == validPassword)
            {
                HttpContext.Session.SetString("Username", account.Username);

                // Ghi cookie luôn (dù có chọn RememberMe hay không)
                CookieOptions option = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(account.RememberMe ? 7 : 1)
                };
                Response.Cookies.Append("Username", account.Username, option);

                TempData["Message"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Message = "Đăng nhập không hợp lệ.";
            return View();
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("Username");

            TempData["Message"] = "Bạn đã đăng xuất thành công.";
            return RedirectToAction("Login", "Account");
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

        // ĐĂNG KÝ (KHÔNG DÙNG db)
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
                // Tạm thời chưa lưu db
                TempData["Message"] = "Đăng ký thành công!";
                return RedirectToAction("Login");
            }

            return View(account);
        }
    }
}
