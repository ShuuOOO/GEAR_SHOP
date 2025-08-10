using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;
using TL4_SHOP.Models;
using TL4_SHOP.Models.ViewModels;

namespace TL4_SHOP.Controllers
{
    public class AccountController : BaseController
    {
        // private readonly _4tlShopContext _context;

        public AccountController(_4tlShopContext context) : base(context)
        {
            // _context = context;
        }

        // Hash password
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        // Tạo reset token
        private string GenerateResetToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        // Kiểm tra email hợp lệ
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
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

            // Kiểm tra rỗng trước
            if (string.IsNullOrEmpty(account?.Username) || string.IsNullOrEmpty(account?.Password))
            {
                ViewBag.Message = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return View(account);
            }

            var hashedPassword = HashPassword(account.Password);

            var user = _context.TaoTaiKhoans
                .FirstOrDefault(u => u.HoTen == account.Username || u.Email == account.Username);
            if (user != null)
            {
                var claims = new List<Claim>();

                if (!string.IsNullOrEmpty(user.HoTen))
                    claims.Add(new Claim(ClaimTypes.Name, user.HoTen));

                claims.Add(new Claim("TaiKhoanId", user.TaiKhoanId.ToString()));

                if (!string.IsNullOrEmpty(user.Email))
                    claims.Add(new Claim(ClaimTypes.Email, user.Email));

                if (!string.IsNullOrEmpty(user.VaiTro))
                    claims.Add(new Claim(ClaimTypes.Role, user.VaiTro));

                // Lưu TaiKhoanId vào session nếu cần
                HttpContext.Session.SetInt32("TaiKhoanId", user.TaiKhoanId);

                // Lấy NhanVienId từ bảng NhanVien dựa trên TaiKhoanId
                var nhanVien = _context.NhanViens.FirstOrDefault(nv => nv.NhanVienId == user.NhanVienId);
                if (nhanVien != null)
                {
                    HttpContext.Session.SetInt32("NhanVienId", nhanVien.NhanVienId);
                }


                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignOutAsync();

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = account.RememberMe,
                        ExpiresUtc = DateTime.UtcNow.AddDays(account.RememberMe ? 7 : 1)
                    });

                TempData["Message"] = "Đăng nhập thành công!";

                // Điều hướng theo vai trò
                if (user.VaiTro == "Admin") return RedirectToAction("Index", "Admin");
                if (user.VaiTro == "Nhân viên quản lý sản phẩm") return RedirectToAction("Index", "QuanLySanPham");
                if (user.VaiTro == "Nhân viên quản lý đơn hàng") return RedirectToAction("Index", "AdminDonHang");
                if (user.VaiTro == "Nhân viên quản lý nhân sự") return RedirectToAction("QuanLyNhanVien", "QuanLyNhanVien");
                if (user.VaiTro == "Nhân viên chăm sóc khách hàng") return RedirectToAction("Index", "ChamSocKhachHang");

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
                // Kiểm tra trùng email
                if (_context.TaoTaiKhoans.Any(u => u.Email == account.Email))
                {
                    ViewBag.Message = "Email đã được sử dụng.";
                    return View(account);
                }

                // Kiểm tra trùng username
                if (_context.TaoTaiKhoans.Any(u => u.HoTen == account.Username))
                {
                    ViewBag.Message = "Tên đăng nhập đã được sử dụng.";
                    return View(account);
                }

                // Kiểm tra trùng số điện thoại
                if (_context.TaoTaiKhoans.Any(u => u.Phone == account.Phone))
                {
                    ViewBag.Message = "Số điện thoại đã được sử dụng.";
                    return View(account);
                }

                try
                {
                    // Hash password
                    var hashedPassword = HashPassword(account.Password);

                    // 👉 Tạo tài khoản + khách hàng chỉ trong 1 lần Add
                    var newUser = new TaoTaiKhoan
                    {
                        HoTen = account.Username,
                        Email = account.Email,
                        Phone = account.Phone,
                        MatKhau = hashedPassword,
                        LoaiTaiKhoan = "KhachHang",
                    };

                    _context.TaoTaiKhoans.Add(newUser);
                    _context.SaveChanges();

                    TempData["Message"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Login");
                }
                catch (Exception)
                {
                    ViewBag.Message = "Có lỗi xảy ra khi đăng ký. Vui lòng thử lại.";
                    return View(account);
                }
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
            try
            {
                var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                if (result?.Principal?.Identity?.IsAuthenticated == true)
                {
                    var username = result.Principal.Identity.Name;
                    var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;

                    if (!string.IsNullOrEmpty(username))
                    {
                        // Kiểm tra user đã tồn tại chưa
                        var existingUser = _context.TaoTaiKhoans.FirstOrDefault(u => u.Email == email);

                        if (existingUser == null && !string.IsNullOrEmpty(email))
                        {
                            // Tạo user mới từ external login
                            var newUser = new TaoTaiKhoan
                            {
                                HoTen = username,
                                Email = email,
                                Phone = "", // External login có thể không có phone
                                MatKhau = HashPassword(Guid.NewGuid().ToString()), // Random password
                                LoaiTaiKhoan = "KhachHang"
                            };

                            _context.TaoTaiKhoans.Add(newUser);
                            _context.SaveChanges();
                        }

                        HttpContext.Session.SetString("Username", username);
                        TempData["Message"] = "Đăng nhập thành công!";
                    }
                }
                else
                {
                    TempData["Message"] = "Đăng nhập thất bại. Vui lòng thử lại.";
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Có lỗi xảy ra khi đăng nhập. Vui lòng thử lại.";
            }

            return RedirectToAction("Index", "Home");
        }

        // Hiển thị trang Quên mật khẩu
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Xử lý gửi yêu cầu quên mật khẩu
        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Message = "Vui lòng nhập email.";
                return View();
            }

            if (!IsValidEmail(email))
            {
                ViewBag.Message = "Email không đúng định dạng.";
                return View();
            }

            var user = _context.TaoTaiKhoans.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Message = "Không tìm thấy tài khoản với email này.";
                return View();
            }

            try
            {
                // Tạo reset token
                var token = GenerateResetToken();
                var resetToken = new TL4_SHOP.Data.PasswordResetToken
                {
                    TaiKhoanId = user.TaiKhoanId,
                    Token = token,
                    ExpiryDate = DateTime.Now.AddHours(1), // Token hết hạn sau 1 giờ
                    IsUsed = false
                };

                _context.PasswordResetTokens.Add(resetToken);
                _context.SaveChanges();

                // Tạo link reset password
                var resetLink = Url.Action("ResetPassword", "Account", new { token = token }, Request.Scheme);

                ViewBag.Message = "Liên kết đặt lại mật khẩu đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư.";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Có lỗi xảy ra khi gửi email. Vui lòng thử lại sau.";
                return View();
            }
        }

        // Hiển thị trang Reset Password
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["Message"] = "Token không hợp lệ.";
                return RedirectToAction("Login");
            }

            // Kiểm tra token có hợp lệ không
            var resetToken = _context.PasswordResetTokens
                 .FirstOrDefault(t => t.Token == token && t.IsUsed == false && t.ExpiryDate > DateTime.Now);

            if (resetToken == null)
            {
                TempData["Message"] = "Token không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("Login");
            }

            ViewBag.Token = token;
            return View();
        }

        // Xử lý Reset Password
        [HttpPost]
        public IActionResult ResetPassword(string token, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Message = "Token không hợp lệ.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Message = "Mật khẩu xác nhận không khớp.";
                ViewBag.Token = token;
                return View();
            }

            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
            {
                ViewBag.Message = "Mật khẩu phải có ít nhất 6 ký tự.";
                ViewBag.Token = token;
                return View();
            }

            try
            {
                // Kiểm tra token
                var resetToken = _context.PasswordResetTokens
                    .Include(t => t.TaiKhoan)
                     .FirstOrDefault(t => t.Token == token && t.IsUsed == false && t.ExpiryDate > DateTime.Now);

                if (resetToken == null)
                {
                    ViewBag.Message = "Token không hợp lệ hoặc đã hết hạn.";
                    return View();
                }

                // Cập nhật mật khẩu
                var user = resetToken.TaiKhoan;
                user.MatKhau = HashPassword(newPassword);

                // Đánh dấu token đã sử dụng
                resetToken.IsUsed = true;

                _context.SaveChanges();

                TempData["Message"] = "Mật khẩu đã được đặt lại thành công. Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Có lỗi xảy ra khi đặt lại mật khẩu. Vui lòng thử lại.";
                ViewBag.Token = token;
                return View();
            }
        }

        // Thêm action này để debug và cập nhật mật khẩu admin
        [HttpGet]
        public IActionResult DebugLogin()
        {
            var adminUser = _context.TaoTaiKhoans.FirstOrDefault(u => u.LoaiTaiKhoan == "Admin");

            if (adminUser != null)
            {
                var result = new
                {
                    HoTen = adminUser.HoTen,
                    Email = adminUser.Email,
                    MatKhauHienTai = adminUser.MatKhau,
                    LoaiTaiKhoan = adminUser.LoaiTaiKhoan,
                    MatKhauHash123 = HashPassword("123")
                };

                return Json(result);
            }

            return Json(new { Message = "Không tìm thấy admin user" });
        }

        // Action để cập nhật mật khẩu thành hash
        [HttpGet]
        public IActionResult UpdateAdminPassword()
        {
            try
            {
                var adminUser = _context.TaoTaiKhoans.FirstOrDefault(u => u.LoaiTaiKhoan == "Admin");

                if (adminUser != null && adminUser.MatKhau == "123")
                {
                    adminUser.MatKhau = HashPassword("123");
                    _context.SaveChanges();

                    return Json(new
                    {
                        Success = true,
                        Message = "Đã cập nhật mật khẩu admin thành hash",
                        NewPassword = adminUser.MatKhau
                    });
                }
                else if (adminUser != null)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Mật khẩu admin đã được hash rồi",
                        CurrentPassword = adminUser.MatKhau
                    });
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Không tìm thấy admin user"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Lỗi: " + ex.Message
                });
            }
        }
        // Action để tạo tài khoản admin
        [HttpGet]
        public IActionResult CreateAdmin()
        {
            try
            {
                // Kiểm tra xem đã có admin chưa
                var existingAdmin = _context.TaoTaiKhoans.FirstOrDefault(u => u.VaiTro == "Admin");

                if (existingAdmin != null)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Đã có tài khoản admin tồn tại",
                        AdminInfo = new
                        {
                            HoTen = existingAdmin.HoTen,
                            Email = existingAdmin.Email,
                            VaiTro = existingAdmin.VaiTro
                        }
                    });
                }

                // Tạo tài khoản admin mới
                var adminUser = new TaoTaiKhoan
                {
                    HoTen = "admin",
                    Email = "admin@shop.com",
                    Phone = "0123456789",
                    MatKhau = HashPassword("123"), // Mật khẩu đã được hash
                    LoaiTaiKhoan = "Admin"
                };

                _context.TaoTaiKhoans.Add(adminUser);
                _context.SaveChanges();

                return Json(new
                {
                    Success = true,
                    Message = "Đã tạo tài khoản admin thành công",
                    AdminInfo = new
                    {
                        HoTen = adminUser.HoTen,
                        Email = adminUser.Email,
                        LoaiTaiKhoan = adminUser.LoaiTaiKhoan,
                        HashedPassword = adminUser.MatKhau
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Lỗi: " + ex.Message
                });
            }
        }

        // Action để reset mật khẩu admin về "123"
        [HttpGet]
        public IActionResult ResetAdminPassword()
        {
            try
            {
                var adminUser = _context.TaoTaiKhoans.FirstOrDefault(u => u.LoaiTaiKhoan == "Admin");

                if (adminUser != null)
                {
                    adminUser.MatKhau = HashPassword("123");
                    _context.SaveChanges();

                    return Json(new
                    {
                        Success = true,
                        Message = "Đã reset mật khẩu admin về '123'",
                        AdminInfo = new
                        {
                            HoTen = adminUser.HoTen,
                            Email = adminUser.Email,
                            NewHashedPassword = adminUser.MatKhau
                        }
                    });
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Không tìm thấy tài khoản admin"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Lỗi: " + ex.Message
                });
            }
        }
        [HttpGet]
        public IActionResult Profile()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return RedirectToAction("Login");

            var taiKhoan = _context.TaoTaiKhoans.FirstOrDefault(t => t.Email == email);
            if (taiKhoan == null) return NotFound();

            var viewModel = new TaiKhoanViewModel
            {
                HoTen = taiKhoan.HoTen,
                Email = taiKhoan.Email,
                Phone = taiKhoan.Phone,
                LoaiTaiKhoan = taiKhoan.LoaiTaiKhoan
            };

            return View(viewModel);
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return RedirectToAction("Login");

            var taiKhoan = _context.TaoTaiKhoans.FirstOrDefault(t => t.Email == email);
            if (taiKhoan == null) return NotFound();

            var viewModel = new ProfileEditViewModel
            {
                HoTen = taiKhoan.HoTen,
                Email = taiKhoan.Email,
                Phone = taiKhoan.Phone
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(ProfileEditViewModel model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return RedirectToAction("Login");

            var taiKhoan = _context.TaoTaiKhoans.FirstOrDefault(t => t.Email == email);
            if (taiKhoan == null) return NotFound();

            // Cập nhật thông tin
            taiKhoan.HoTen = model.HoTen;
            taiKhoan.Phone = model.Phone;

            // Nếu muốn đổi mật khẩu
            if (!string.IsNullOrEmpty(model.CurrentPassword) &&
                !string.IsNullOrEmpty(model.NewPassword) &&
                !string.IsNullOrEmpty(model.ConfirmPassword))
            {
                var currentHashed = HashPassword(model.CurrentPassword);
                if (taiKhoan.MatKhau != currentHashed)
                {
                    ViewBag.Message = "Mật khẩu hiện tại không đúng.";
                    return View(model);
                }

                if (model.NewPassword != model.ConfirmPassword)
                {
                    ViewBag.Message = "Mật khẩu xác nhận không khớp.";
                    return View(model);
                }

                if (model.NewPassword.Length < 6)
                {
                    ViewBag.Message = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                    return View(model);
                }

                taiKhoan.MatKhau = HashPassword(model.NewPassword);
            }

            _context.SaveChanges();

            TempData["Message"] = "Cập nhật thông tin thành công.";
            return RedirectToAction("Profile");
        }

    }
}