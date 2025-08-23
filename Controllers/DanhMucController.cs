using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;

namespace TL4_SHOP.Controllers
{
    public class DanhMucController : Controller
    {
        private readonly _4tlShopContext _context;

        public DanhMucController(_4tlShopContext context)
        {
            _context = context;
        }

        public IActionResult Menu()
        {
            // Lấy danh mục cha và kèm con
            var danhMucs = _context.DanhMucSanPhams
                .Where(dm => dm.DanhMucChaId == null)
                .Include(dm => dm.DanhMucCon)
                .ToList();

            return PartialView("_MenuDanhMuc", danhMucs);
        }
    }
}
