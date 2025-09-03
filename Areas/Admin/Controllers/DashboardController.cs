using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GEAR_SHOP.Models.ViewModels;
using TL4_SHOP.Data;

namespace GEAR_SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AnyStaff")]
    public class DashboardController : Controller
    {
        private readonly _4tlShopContext _context;
        public DashboardController(_4tlShopContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var monthEnd = monthStart.AddMonths(1);

            var vm = new DashboardViewModel
            {
                TongSanPham = await _context.SanPhams.CountAsync(),
                TongDonHang = await _context.DonHangs.CountAsync(),
                TongKhachHang = await _context.KhachHangs.CountAsync(),

                // Doanh thu tháng = TongTien + PhiVanChuyen (đơn trạng thái = 4)
                DoanhThuThang = await _context.DonHangs
                    .Where(d => d.TrangThaiId == 4 &&
                                d.NgayDatHang >= monthStart && d.NgayDatHang < monthEnd)
                    .Select(d => d.TongTien + d.PhiVanChuyen)    // <- KHÔNG dùng ??
                    .SumAsync(),
            };

            // Đếm theo trạng thái
            vm.DonChoXacNhan = await _context.DonHangs.CountAsync(d => d.TrangThaiId == 1);
            vm.DonDaXacNhan = await _context.DonHangs.CountAsync(d => d.TrangThaiId == 2);
            vm.DonDangGiao = await _context.DonHangs.CountAsync(d => d.TrangThaiId == 3);
            vm.DonGiaoThanhCong = await _context.DonHangs.CountAsync(d => d.TrangThaiId == 4);
            vm.DonDaHuy = await _context.DonHangs.CountAsync(d => d.TrangThaiId == 5);

            // Doanh thu 7 ngày gần nhất (đơn trạng thái = 4)
            var today = DateTime.Today;
            var from = today.AddDays(-6);

            var last7 = await _context.DonHangs
                .Where(d => d.TrangThaiId == 4 &&
                            d.NgayDatHang >= from && d.NgayDatHang < today.AddDays(1))
                .GroupBy(d => d.NgayDatHang.Date)
                .Select(g => new
                {
                    Ngay = g.Key,
                    DoanhThu = g.Sum(x => x.TongTien + x.PhiVanChuyen) // <- KHÔNG dùng ??
                })
                .ToListAsync();

            // Đổ dữ liệu theo thứ tự ngày
            for (int i = 0; i < 7; i++)
            {
                var d = from.AddDays(i);
                var row = last7.FirstOrDefault(x => x.Ngay == d.Date);
                vm.NgayLabels.Add(d.ToString("dd/MM"));
                vm.DoanhThuNgay.Add(row?.DoanhThu ?? 0m); // row có thể null, nên ?? ở đây là hợp lệ
            }

            return View(vm);
        }
    }
}