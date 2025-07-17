using Microsoft.AspNetCore.Mvc;
using TL4_SHOP.Data;
using TL4_SHOP.ViewModels;

namespace TL4_SHOP.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly Tl4ShopContext db;

        public MenuLoaiViewComponent(Tl4ShopContext context) => db = context;

        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(lo => new MenuLoaiVM
            {
                Maloai = lo.MaLoai,
                TenLoai = lo.TenLoai,
                SoLuong = lo.SoLuong
            });

            return View(data); // Default.cshtml
            // return View("Default", data);
        }
    }
}
