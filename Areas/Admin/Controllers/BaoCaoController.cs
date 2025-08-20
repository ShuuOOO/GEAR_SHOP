using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GEAR_SHOP.Models.ViewModels;
using TL4_SHOP.Data;

namespace TL4_SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BaoCaoController : Controller
    {
        private readonly _4tlShopContext _context;
        public BaoCaoController(_4tlShopContext context) => _context = context;

        public async Task<IActionResult> Index(string mode = "day", DateTime? from = null, DateTime? to = null, int? year = null)
        {
            var vm = new BaoCaoDoanhThuVM { Mode = mode, From = from, To = to, Year = year };

            if (mode == "day")
            {
                var f = from ?? DateTime.Today.AddDays(-29);
                var t = to ?? DateTime.Today;
                var f0 = DateOnly.FromDateTime(f.Date);
                var t0 = DateOnly.FromDateTime(t.Date);

                var rows = await _context.DoanhThuTheoNgays
                    .Where(x => x.Ngay >= f0 && x.Ngay <= t0)
                    .OrderBy(x => x.Ngay)
                    .AsNoTracking()
                    .ToListAsync();

                vm.Labels = rows.Select(r => r.Ngay.ToDateTime(TimeOnly.MinValue).ToString("dd/MM")).ToList();
                vm.Values = rows.Select(r => r.TongDoanhThu ?? 0m).ToList();

                vm.TongSoDonHang = rows.Sum(r => r.TongSoDonHang ?? 0);
                vm.TongSoLuong = rows.Sum(r => r.TongSoLuong ?? 0);
                vm.TongDoanhThu = rows.Sum(r => r.TongDoanhThu ?? 0m);
                vm.TongLoiNhuan = rows.Sum(r => r.LoiNhuan ?? 0m);
            }
            else if (mode == "month")
            {
                int y = year ?? DateTime.Today.Year;
                var rows = await _context.DoanhThuTheoThangs
                    .Where(x => x.Nam == y)
                    .OrderBy(x => x.Thang)
                    .AsNoTracking()
                    .ToListAsync();

                vm.Labels = Enumerable.Range(1, 12).Select(m => $"{m:00}/{y}").ToList();
                var map = rows.ToDictionary(r => r.Thang, r => r);
                vm.Values = Enumerable.Range(1, 12).Select(m => (map.ContainsKey(m) ? map[m].TongDoanhThu : 0m) ?? 0m).ToList();

                vm.TongSoDonHang = rows.Sum(r => r.TongSoDonHang ?? 0);
                vm.TongSoLuong = rows.Sum(r => r.TongSoLuong ?? 0);
                vm.TongDoanhThu = rows.Sum(r => r.TongDoanhThu ?? 0m);
                vm.TongLoiNhuan = rows.Sum(r => r.LoiNhuan ?? 0m);
                vm.Year = y;
            }
            else // year
            {
                var rows = await _context.DoanhThuTheoNams
                    .OrderBy(x => x.Nam)
                    .AsNoTracking()
                    .ToListAsync();

                vm.Labels = rows.Select(r => r.Nam.ToString()).ToList();
                vm.Values = rows.Select(r => r.TongDoanhThu ?? 0m).ToList();

                vm.TongSoDonHang = rows.Sum(r => r.TongSoDonHang ?? 0);
                vm.TongSoLuong = rows.Sum(r => r.TongSoLuong ?? 0);
                vm.TongDoanhThu = rows.Sum(r => r.TongDoanhThu ?? 0m);
                vm.TongLoiNhuan = rows.Sum(r => r.LoiNhuan ?? 0m);
            }

            return View(vm);
        }
    }
}
