using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;
using TL4_SHOP.Models.ViewModels;
using System.Security.Cryptography;

namespace TL4_SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOrHRManager")]
    public class NhanViensController : Controller
    {
        private readonly _4tlShopContext _context;

        public NhanViensController(_4tlShopContext context)
        {
            _context = context;
        }

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
            var dsNhanVien = _context.TaoTaiKhoans
                .Where(t => t.LoaiTaiKhoan == "NhanVien")
                .AsQueryable();

            if (!string.IsNullOrEmpty(TuKhoa))
            {
                dsNhanVien = dsNhanVien.Where(t =>
                    t.TaiKhoanId.ToString().Contains(TuKhoa) ||
                    t.HoTen.Contains(TuKhoa) ||
                    t.Email.Contains(TuKhoa) ||
                    t.Phone.Contains(TuKhoa));
            }

            return View(dsNhanVien.ToList());
        }


        // Details
        public IActionResult Details(int id)
        {
            var nv = _context.TaoTaiKhoans
                .FirstOrDefault(m => m.TaiKhoanId == id && m.LoaiTaiKhoan == "NhanVien");

            if (nv == null)
            {
                return NotFound();
            }

            return View(nv);
        }

        // GET: Create
        public IActionResult Create()
        {
            return View(new TaiKhoanCreateViewModel());
        }
        // POST: Create
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
                LoaiTaiKhoan = "NhanVien",
                VaiTro = model.VaiTro
            };

            _context.TaoTaiKhoans.Add(entity);
            _context.SaveChanges();

            TempData["ThongBao"] = "Thêm nhân viên thành công !";
            return RedirectToAction(nameof(Index));
        }

        // GET: Edit
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

            ViewBag.Id = id;
            return View(vm);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TaiKhoanEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Id = id;
                return View(model);
            }

            var taiKhoan = _context.TaoTaiKhoans.Find(id);
            if (taiKhoan == null) return NotFound();

            // Kiểm tra email
            bool emailExists = _context.TaoTaiKhoans
                .Any(x => x.Email == model.Email && x.TaiKhoanId != id);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng bởi tài khoản khác.");
                return View(model);
            }

            // Kiểm tra phone
            bool phoneExists = _context.TaoTaiKhoans
                .Any(x => x.Phone == model.Phone && x.TaiKhoanId != id);
            if (phoneExists)
            {
                ModelState.AddModelError("Phone", "Số điện thoại đã được sử dụng bởi tài khoản khác.");
                return View(model);
            }

            // Cập nhật thông tin
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

            TempData["ThongBao"] = "Cập nhật nhân viên thành công !";
            return RedirectToAction(nameof(Index));
        }

        // GET: Delete
        public IActionResult Delete(int id)
        {
            var nv = _context.TaoTaiKhoans
                .FirstOrDefault(m => m.TaiKhoanId == id && m.LoaiTaiKhoan == "NhanVien");

            if (nv == null)
            {
                return NotFound();
            }

            return View(nv);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var nv = _context.TaoTaiKhoans.Find(id);

            if (nv != null && nv.LoaiTaiKhoan == "NhanVien")
            {
                _context.TaoTaiKhoans.Remove(nv);
                _context.SaveChanges();
            }

            TempData["ThongBao"] = "Xóa nhân viên thành công !";
            return RedirectToAction(nameof(Index));
        }
    }
}
