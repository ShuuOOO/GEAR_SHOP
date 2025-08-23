using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;
using TL4_SHOP.Models.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace TL4_SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class KhachHangsController : Controller
    {
        private readonly _4tlShopContext _context;

        public KhachHangsController(_4tlShopContext context)
        {
            _context = context;
        }



        // Hàm hash
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Index
        public IActionResult Index(string? TuKhoa)
        {
            var dsKhachHang = _context.TaoTaiKhoans
                .Include(t => t.KhachHang)
                .Where(t => t.LoaiTaiKhoan == "KhachHang")
                .AsQueryable();

            // Nếu có nhập từ khóa thì lọc theo họ tên, email, phone
            if (!string.IsNullOrEmpty(TuKhoa))
            {
                dsKhachHang = dsKhachHang.Where(t =>
                    t.TaiKhoanId.ToString().Contains(TuKhoa) || // ép sang string
                    t.HoTen.Contains(TuKhoa) ||
                    t.Email.Contains(TuKhoa) ||
                    t.Phone.Contains(TuKhoa));
            }

            return View(dsKhachHang.ToList());
        }

        // Details
        public IActionResult Details(int id)
        {
            var kh = _context.TaoTaiKhoans
                .Include(t => t.KhachHang)
                .FirstOrDefault(m => m.TaiKhoanId == id && m.LoaiTaiKhoan == "KhachHang");

            if (kh == null)
            {
                return NotFound();
            }

            return View(kh);
        }

        // GET: Create
        public IActionResult Create()
        {
            return View(new TaiKhoanCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TaiKhoanCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_context.TaoTaiKhoans.Any(t => t.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                return View(model);
            }

            if (_context.TaoTaiKhoans.Any(t => t.Phone == model.Phone))
            {
                ModelState.AddModelError("Phone", "Số điện thoại này đã được sử dụng.");
                return View(model);
            }

            var entity = new TaoTaiKhoan
            {
                HoTen = model.HoTen,
                Email = model.Email,
                Phone = model.Phone,
                MatKhau = HashPassword(model.MatKhau), // Hash mật khẩu
                LoaiTaiKhoan = "KhachHang",
                VaiTro = model.VaiTro
            };

            _context.TaoTaiKhoans.Add(entity);
            _context.SaveChanges();

            TempData["ThongBao"] = "Thêm khách hàng thành công !";
            return RedirectToAction(nameof(Index));
        }


        // GET: Chỉnh sửa
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var taiKhoan = _context.TaoTaiKhoans.Find(id);
            if (taiKhoan == null) return NotFound();

            var vm = new TaiKhoanEditViewModel
            {
                HoTen = taiKhoan.HoTen,
                Email = taiKhoan.Email,
                Phone = taiKhoan.Phone,
                LoaiTaiKhoan = taiKhoan.LoaiTaiKhoan,
                VaiTro = taiKhoan.VaiTro
            };

            ViewBag.Id = id; // để truyền vào hidden field
            return View(vm);
        }

        // POST: Chỉnh sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TaiKhoanEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Id = id; // giữ lại id nếu reload
                return View(model);
            }

            var taiKhoan = _context.TaoTaiKhoans.Find(id);
            if (taiKhoan == null) return NotFound();

            // Kiểm tra email đã tồn tại ở tài khoản khác chưa
            bool emailExists = _context.TaoTaiKhoans
                .Any(x => x.Email == model.Email && x.TaiKhoanId != id);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng bởi tài khoản khác.");
                return View(model);
            }

            // Kiểm tra phone đã tồn tại ở tài khoản khác chưa
            bool phoneExists = _context.TaoTaiKhoans
                .Any(x => x.Phone == model.Phone && x.TaiKhoanId != id);
            if (phoneExists)
            {
                ModelState.AddModelError("Phone", "Số điện thoại đã được sử dụng bởi tài khoản khác.");
                return View(model);
            }

            // Gán giá trị
            taiKhoan.HoTen = model.HoTen;
            taiKhoan.Email = model.Email;
            taiKhoan.Phone = model.Phone;
            taiKhoan.LoaiTaiKhoan = model.LoaiTaiKhoan;
            taiKhoan.VaiTro = model.VaiTro;

            if (!string.IsNullOrEmpty(model.MatKhau))
            {
                // Hash mật khẩu và gán vào entity
                taiKhoan.MatKhau = HashPassword(model.MatKhau);
            }

            _context.Update(taiKhoan);
            _context.SaveChanges();

            TempData["ThongBao"] = "Cập nhật khách hàng thành công !";
            return RedirectToAction(nameof(Index));
        }



        // GET: Delete
        public IActionResult Delete(int id)
        {
            var kh = _context.TaoTaiKhoans
                .FirstOrDefault(m => m.TaiKhoanId == id && m.LoaiTaiKhoan == "KhachHang");

            if (kh == null)
            {
                return NotFound();
            }

            return View(kh);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var kh = _context.TaoTaiKhoans.Find(id);

            if (kh != null && kh.LoaiTaiKhoan == "KhachHang")
            {
                _context.TaoTaiKhoans.Remove(kh);
                _context.SaveChanges();
            }
            TempData["ThongBao"] = "Xóa khách hàng thành công !";
            return RedirectToAction(nameof(Index));
        }
    }
}
