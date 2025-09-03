using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GEAR_SHOP.Models.ViewModels;
using TL4_SHOP.Data;

namespace TL4_SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class BaoCaoController : Controller
    {
        private readonly _4tlShopContext _context;
        public BaoCaoController(_4tlShopContext context) => _context = context;

        public async Task<IActionResult> Index(string mode = "day", DateTime? from = null, DateTime? to = null, int? year = null)
        {
            var vm = new BaoCaoDoanhThuVM { Mode = mode, From = from, To = to, Year = year };

            // Đơn hàng đã hoàn tất (TrangThaiId = 4)
            var orders = _context.DonHangs.AsNoTracking()
                .Where(d => d.TrangThaiId == 4)
                .Select(d => new
                {
                    d.DonHangId,
                    Day = d.NgayDatHang.Date,
                    Month = d.NgayDatHang.Month,
                    Year = d.NgayDatHang.Year,
                    DoanhThu = d.TongTien + d.PhiVanChuyen
                });

            // ------------------- THEO NGÀY -------------------
            if (mode == "day")
            {
                var f = from?.Date ?? DateTime.Today.AddDays(-29);
                var t = (to?.Date ?? DateTime.Today).AddDays(1).AddTicks(-1);

                var series = await orders
                    .Where(x => x.Day >= f && x.Day <= t)
                    .GroupBy(x => x.Day)
                    .Select(g => new
                    {
                        Key = g.Key,
                        Don = g.Select(x => x.DonHangId).Distinct().Count(),
                        DoanhThu = g.Sum(x => x.DoanhThu)
                    })
                    .OrderBy(x => x.Key)
                    .ToListAsync();

                // Tổng số lượng bán
                var soLuongBan = await (from ct in _context.ChiTietDonHangs.AsNoTracking()
                                        join d in _context.DonHangs.AsNoTracking() on ct.DonHangId equals d.DonHangId
                                        where d.TrangThaiId == 4 && d.NgayDatHang >= f && d.NgayDatHang <= t
                                        select (int?)ct.SoLuong).SumAsync() ?? 0;

                // Tính chi phí vốn theo giá nhập bình quân
                var chiTietBans = await (from ct in _context.ChiTietDonHangs.AsNoTracking()
                                         join d in _context.DonHangs.AsNoTracking() on ct.DonHangId equals d.DonHangId
                                         where d.TrangThaiId == 4 && d.NgayDatHang >= f && d.NgayDatHang <= t
                                         select new { ct.SanPhamId, ct.SoLuong }).ToListAsync();

                decimal tongChiPhi = 0;
                foreach (var item in chiTietBans)
                {
                    var queryNhap = _context.ChiTietNhapHangs.Where(n => n.SanPhamId == item.SanPhamId);
                    var tongSL = await queryNhap.SumAsync(n => (int?)n.SoLuong) ?? 0;
                    if (tongSL > 0)
                    {
                        var tongTienNhap = await queryNhap.SumAsync(n => (decimal?)n.SoLuong * n.DonGiaNhap) ?? 0;
                        var giaNhapTb = tongTienNhap / tongSL;
                        tongChiPhi += giaNhapTb * item.SoLuong;
                    }
                }

                vm.Labels = series.Select(r => r.Key.ToString("dd/MM")).ToList();
                vm.Values = series.Select(r => r.DoanhThu).ToList();
                vm.TongSoDonHang = series.Sum(r => r.Don);
                vm.TongSoLuong = soLuongBan;
                vm.TongDoanhThu = series.Sum(r => r.DoanhThu);
                vm.TongLoiNhuan = vm.TongDoanhThu - tongChiPhi;
                return View(vm);
            }

            // ------------------- THEO THÁNG -------------------
            if (mode == "month")
            {
                int y = year ?? DateTime.Today.Year;

                var series = await orders
                    .Where(x => x.Year == y)
                    .GroupBy(x => x.Month)
                    .Select(g => new
                    {
                        Key = g.Key,
                        Don = g.Select(x => x.DonHangId).Distinct().Count(),
                        DoanhThu = g.Sum(x => x.DoanhThu)
                    })
                    .OrderBy(x => x.Key)
                    .ToListAsync();

                var soLuongBan = await (from ct in _context.ChiTietDonHangs.AsNoTracking()
                                        join d in _context.DonHangs.AsNoTracking() on ct.DonHangId equals d.DonHangId
                                        where d.TrangThaiId == 4 && d.NgayDatHang.Year == y
                                        select (int?)ct.SoLuong).SumAsync() ?? 0;

                // Tính chi phí vốn
                var chiTietBans = await (from ct in _context.ChiTietDonHangs.AsNoTracking()
                                         join d in _context.DonHangs.AsNoTracking() on ct.DonHangId equals d.DonHangId
                                         where d.TrangThaiId == 4 && d.NgayDatHang.Year == y
                                         select new { ct.SanPhamId, ct.SoLuong }).ToListAsync();

                decimal tongChiPhi = 0;
                foreach (var item in chiTietBans)
                {
                    var queryNhap = _context.ChiTietNhapHangs.Where(n => n.SanPhamId == item.SanPhamId);
                    var tongSL = await queryNhap.SumAsync(n => (int?)n.SoLuong) ?? 0;
                    if (tongSL > 0)
                    {
                        var tongTienNhap = await queryNhap.SumAsync(n => (decimal?)n.SoLuong * n.DonGiaNhap) ?? 0;
                        var giaNhapTb = tongTienNhap / tongSL;
                        tongChiPhi += giaNhapTb * item.SoLuong;
                    }
                }

                vm.Labels = series.Select(r => $"Thg {r.Key:00}").ToList();
                vm.Values = series.Select(r => r.DoanhThu).ToList();
                vm.TongSoDonHang = series.Sum(r => r.Don);
                vm.TongSoLuong = soLuongBan;
                vm.TongDoanhThu = series.Sum(r => r.DoanhThu);
                vm.TongLoiNhuan = vm.TongDoanhThu - tongChiPhi;
                return View(vm);
            }

            // ------------------- THEO NĂM -------------------
            {
                var series = await orders
                    .GroupBy(x => x.Year)
                    .Select(g => new
                    {
                        Key = g.Key,
                        Don = g.Select(x => x.DonHangId).Distinct().Count(),
                        DoanhThu = g.Sum(x => x.DoanhThu)
                    })
                    .OrderBy(x => x.Key)
                    .ToListAsync();

                var soLuongBan = await (from ct in _context.ChiTietDonHangs.AsNoTracking()
                                        join d in _context.DonHangs.AsNoTracking() on ct.DonHangId equals d.DonHangId
                                        where d.TrangThaiId == 4
                                        select (int?)ct.SoLuong).SumAsync() ?? 0;

                // Tính chi phí vốn
                var chiTietBans = await (from ct in _context.ChiTietDonHangs.AsNoTracking()
                                         join d in _context.DonHangs.AsNoTracking() on ct.DonHangId equals d.DonHangId
                                         where d.TrangThaiId == 4
                                         select new { ct.SanPhamId, ct.SoLuong }).ToListAsync();

                decimal tongChiPhi = 0;
                foreach (var item in chiTietBans)
                {
                    var queryNhap = _context.ChiTietNhapHangs.Where(n => n.SanPhamId == item.SanPhamId);
                    var tongSL = await queryNhap.SumAsync(n => (int?)n.SoLuong) ?? 0;
                    if (tongSL > 0)
                    {
                        var tongTienNhap = await queryNhap.SumAsync(n => (decimal?)n.SoLuong * n.DonGiaNhap) ?? 0;
                        var giaNhapTb = tongTienNhap / tongSL;
                        tongChiPhi += giaNhapTb * item.SoLuong;
                    }
                }

                vm.Labels = series.Select(r => r.Key.ToString()).ToList();
                vm.Values = series.Select(r => r.DoanhThu).ToList();
                vm.TongSoDonHang = series.Sum(r => r.Don);
                vm.TongSoLuong = soLuongBan;
                vm.TongDoanhThu = series.Sum(r => r.DoanhThu);
                vm.TongLoiNhuan = vm.TongDoanhThu - tongChiPhi;
                return View(vm);
            }
        }
    }
}
