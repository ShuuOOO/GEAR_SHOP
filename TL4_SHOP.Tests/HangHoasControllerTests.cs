using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Controllers;
using TL4_SHOP.Data;
using Xunit;

namespace TL4_SHOP.Tests
{
    public class HangHoasControllerTests
    {
        private static Tl4ShopContext GetContext()
        {
            var options = new DbContextOptionsBuilder<Tl4ShopContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new Tl4ShopContext(options);
            context.HangHoas.AddRange(
                new HangHoa { Id = 1, Ten = "HH1", GiaSanPham = 10 },
                new HangHoa { Id = 2, Ten = "HH2", GiaSanPham = 20 }
            );
            context.SaveChanges();
            return context;
        }

        [Fact]
        public async Task Index_ReturnsAllHangHoas()
        {
            using var context = GetContext();
            var controller = new HangHoasController(context);

            var result = await controller.Index() as ViewResult;

            var model = Assert.IsAssignableFrom<List<HangHoa>>(result?.Model);
            Assert.Equal(2, model.Count);
        }
    }
}
