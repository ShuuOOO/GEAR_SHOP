using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;
using TL4_SHOP.ViewComponents;
using TL4_SHOP.ViewModels;
using Xunit;

namespace TL4_SHOP.Tests
{
    public class MenuLoaiViewComponentTests
    {
        private static Tl4ShopContext GetContext()
        {
            var options = new DbContextOptionsBuilder<Tl4ShopContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new Tl4ShopContext(options);
            var hh1 = new HangHoa { Id = 1, Ten = "HH1", GiaSanPham = 10 };
            var hh2 = new HangHoa { Id = 2, Ten = "HH2", GiaSanPham = 20 };
            context.HangHoas.AddRange(hh1, hh2);
            context.Loais.AddRange(
                new Loai { MaLoai = 1, TenLoai = "Loai1", TenLoaiAlias = "loai1", SoLuong = 5, MaLoaiNavigation = hh1 },
                new Loai { MaLoai = 2, TenLoai = "Loai2", TenLoaiAlias = "loai2", SoLuong = 3, MaLoaiNavigation = hh2 }
            );
            context.SaveChanges();
            return context;
        }

        [Fact]
        public void Invoke_ReturnsLoaiMenu()
        {
            using var context = GetContext();
            var component = new MenuLoaiViewComponent(context);

            var result = component.Invoke() as ViewViewComponentResult;

            var model = Assert.IsAssignableFrom<IEnumerable<MenuLoaiVM>>(result?.ViewData.Model);
            Assert.Equal(2, model.Count());
        }
    }
}
