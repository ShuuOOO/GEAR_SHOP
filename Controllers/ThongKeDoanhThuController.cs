using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;
using TL4_SHOP.Models.ViewModels;

namespace TL4_SHOP.Controllers
{
    [Authorize(Roles = "Admin, Nhân viên quản lý đơn hàng")]
    public class ThongKeDoanhThuController : Controller
    {
        private readonly _4tlShopContext _context;

        public ThongKeDoanhThuController(_4tlShopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ThongKeDoanhThu()
        {
            var viewModel = new ThongKeViewModel
            {
                DoanhThuNgay = await _context.DoanhThuTheoNgays
                    .OrderByDescending(d => d.Ngay)
                    .ToListAsync(),

                DoanhThuThang = await _context.DoanhThuTheoThangs
                    .OrderByDescending(d => d.Nam)
                    .ThenByDescending(d => d.Thang)
                    .ToListAsync(),

                DoanhThuNam = await _context.DoanhThuTheoNams
                    .OrderByDescending(d => d.Nam)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
}
