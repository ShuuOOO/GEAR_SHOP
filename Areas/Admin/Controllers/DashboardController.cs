using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GEAR_SHOP.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using TL4_SHOP.Data;

namespace GEAR_SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    // Nếu bạn chưa dùng ASP.NET Identity Role, có thể tạm bỏ Authorize để test:
    //[Authorize(Roles = "Admin,Nhân viên quản lý nhân sự")]
    public class DashboardController : Controller
    {
        private readonly _4tlShopContext _context; // đổi đúng tên DbContext thực tế của bạn
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

                // Doanh thu tháng: lấy TongTien + PhiVanChuyen (nếu muốn chỉ TongTien thì bỏ PhiVanChuyen)
                DoanhThuThang = await _context.DonHangs
                    .Where(d => d.NgayDatHang >= monthStart && d.NgayDatHang < monthEnd)
                    .SumAsync(d => (decimal?)d.TongTien + (decimal?)d.PhiVanChuyen) ?? 0
            };

            // Đếm theo trạng thái
            vm.DonChoXacNhan = await _context.DonHangs.CountAsync(d => d.TrangThaiId == 1);
            vm.DonDaXacNhan = await _context.DonHangs.CountAsync(d => d.TrangThaiId == 2);
            vm.DonDangGiao = await _context.DonHangs.CountAsync(d => d.TrangThaiId == 3);
            vm.DonGiaoThanhCong = await _context.DonHangs.CountAsync(d => d.TrangThaiId == 4);
            vm.DonDaHuy = await _context.DonHangs.CountAsync(d => d.TrangThaiId == 5);


            // Biểu đồ doanh thu 7 ngày gần nhất
            var today = DateTime.Today;
            var from = today.AddDays(-6);

            // Cách 1: đọc trực tiếp từ DonHang (an toàn khi bảng tổng hợp chưa đủ dữ liệu)
            var last7 = await _context.DonHangs
                .Where(d => d.NgayDatHang >= from && d.NgayDatHang < today.AddDays(1))
                .GroupBy(d => d.NgayDatHang.Date)
                .Select(g => new {
                    Ngay = g.Key,
                    DoanhThu = g.Sum(x => x.TongTien + x.PhiVanChuyen)
                })
                .ToListAsync();

            // Cách 2 (tùy chọn): nếu bạn maintain bảng DoanhThuTheoNgay đều đặn, dùng bảng này sẽ nhẹ hơn
            // var last7 = await _context.DoanhThuTheoNgays
            //     .Where(x => x.Ngay >= from && x.Ngay <= today)
            //     .OrderBy(x => x.Ngay)
            //     .Select(x => new { Ngay = x.Ngay, DoanhThu = x.TongDoanhThu ?? 0 })
            //     .ToListAsync();

            // Đổ dữ liệu theo thứ tự ngày
            for (int i = 0; i < 7; i++)
            {
                var d = from.AddDays(i);
                var row = last7.FirstOrDefault(x => x.Ngay.Date == d.Date);
                vm.NgayLabels.Add(d.ToString("dd/MM"));
                vm.DoanhThuNgay.Add(row?.DoanhThu ?? 0);
            }

            return View(vm);
        }
    }
}

