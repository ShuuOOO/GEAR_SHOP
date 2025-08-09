using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;

namespace TL4_SHOP.Controllers
{
    public class SanPhamController : BaseController
    {
        private readonly _4tlShopContext _context;

        public SanPhamController(_4tlShopContext context) : base(context)
        {
            _context = context;
        }

        // File: Controllers/SanPhamController.cs
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var sanPham = await _context.SanPhams
                .Include(sp => sp.DanhMuc)
                .Include(sp => sp.NhaCungCap)
                .FirstOrDefaultAsync(sp => sp.SanPhamId == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            return View("~/Views/SanPhams/Details.cshtml", sanPham);
        }
    }
}
