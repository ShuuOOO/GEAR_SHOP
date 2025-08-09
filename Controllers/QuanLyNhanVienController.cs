using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;
using TL4_SHOP.Models.ViewModels;

namespace TL4_SHOP.Controllers
{
    [Authorize(Roles = "Nhân viên quản lý nhân sự, Admin")]
    public class QuanLyNhanVienController : Controller
    {
        private readonly _4tlShopContext _context;

        public QuanLyNhanVienController(_4tlShopContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách tài khoản
        public async Task<IActionResult> QuanLyNhanVien()
        {
            var taiKhoans = await _context.TaoTaiKhoans
                .Where(tk => tk.NhanVienId != null)
                .ToListAsync();

            return View(taiKhoans);
        }

        // GET: Tạo mới tài khoản
        [HttpGet]
        public IActionResult Create()
        {
            return View(new TaiKhoanCreateViewModel());
        }

        // POST: Tạo mới tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaiKhoanCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var taiKhoan = new TaoTaiKhoan
                {
                    HoTen = model.HoTen,
                    Email = model.Email,
                    Phone = model.Phone,
                    MatKhau = HashPassword(model.MatKhau),
                    LoaiTaiKhoan = model.LoaiTaiKhoan,
                    VaiTro = model.VaiTro
                };

                _context.TaoTaiKhoans.Add(taiKhoan);
                await _context.SaveChangesAsync();
                return RedirectToAction("QuanLyNhanVien");
            }

            return View(model);
        }

        // GET: Sửa tài khoản
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tk = await _context.TaoTaiKhoans.FindAsync(id);
            if (tk == null) return NotFound();

            var viewModel = new TaiKhoanEditViewModel
            {
                TaiKhoanId = tk.TaiKhoanId,
                HoTen = tk.HoTen,
                Email = tk.Email,
                Phone = tk.Phone,
                LoaiTaiKhoan = tk.LoaiTaiKhoan,
                //VaiTro = tk.VaiTro
            };

            return View(viewModel);
        }

        // POST: Lưu sửa tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaiKhoanEditViewModel model)
        {
            if (id != model.TaiKhoanId)
                return NotFound();

            if (ModelState.IsValid)
            {
                var taiKhoan = await _context.TaoTaiKhoans.FindAsync(id);
                if (taiKhoan == null) return NotFound();

                taiKhoan.HoTen = model.HoTen;
                taiKhoan.Email = model.Email;
                taiKhoan.Phone = model.Phone;
                taiKhoan.LoaiTaiKhoan = model.LoaiTaiKhoan;
                //taiKhoan.VaiTro = model.VaiTro;

                if (!string.IsNullOrEmpty(model.MatKhau))
                {
                    taiKhoan.MatKhau = HashPassword(model.MatKhau);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("QuanLyNhanVien");
            }

            return View(model);
        }

        // Hàm băm mật khẩu
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // GET: Xác nhận xóa tài khoản
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var taiKhoan = await _context.TaoTaiKhoans
                .FirstOrDefaultAsync(t => t.TaiKhoanId == id);

            if (taiKhoan == null) return NotFound();

            return View(taiKhoan); // Trả về view xác nhận xóa
        }

        // POST: Xóa tài khoản
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taiKhoan = await _context.TaoTaiKhoans.FindAsync(id);
            if (taiKhoan == null) return NotFound();

            _context.TaoTaiKhoans.Remove(taiKhoan);
            await _context.SaveChangesAsync();

            return RedirectToAction("QuanLyNhanVien");
        }

    }
}
