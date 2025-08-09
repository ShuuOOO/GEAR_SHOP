using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;

namespace TL4_SHOP.Controllers
{
    [Authorize(Roles = "Nhân viên quản lý sản phẩm, Admin")]
    public class NhaCungCapController : Controller
    {
        private readonly _4tlShopContext _context;

        public NhaCungCapController(_4tlShopContext context)
        {
            _context = context;
        }

        // Danh sách
        public IActionResult Index()
        {
            var danhSachNCC = _context.NhaCungCaps.ToList();
            return View(danhSachNCC);
        }

        // Thêm - GET
        public IActionResult Create()
        {
            return View();
        }

        // Thêm - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NhaCungCap ncc)
        {
            if (ModelState.IsValid)
            {
                _context.NhaCungCaps.Add(ncc);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(ncc);
        }

        // Sửa - GET
        public IActionResult Edit(int id)
        {
            var ncc = _context.NhaCungCaps.Find(id);
            if (ncc == null)
                return NotFound();
            return View(ncc);
        }

        // Sửa - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, NhaCungCap ncc)
        {
            if (id != ncc.NhaCungCapId)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(ncc);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(ncc);
        }

        // Xóa - GET
        public IActionResult Delete(int id)
        {
            var ncc = _context.NhaCungCaps.Find(id);
            if (ncc == null)
                return NotFound();
            return View(ncc);
        }

        // Xóa - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var ncc = _context.NhaCungCaps.Find(id);
            if (ncc != null)
            {
                _context.NhaCungCaps.Remove(ncc);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
