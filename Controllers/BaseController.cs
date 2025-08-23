using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;
using TL4_SHOP.Models.ViewModels;

namespace TL4_SHOP.Controllers
{
    public class BaseController : Controller
    {
        protected readonly _4tlShopContext _context;

        public BaseController(_4tlShopContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //  ÉP KHỞI TẠO SESSION SỚM
            var session = context.HttpContext.Session;
            if (string.IsNullOrEmpty(session.GetString("Initialized")))
            {
                session.SetString("Initialized", "true");
            }

            // Danh mục menu như cũ
            var danhMuc = _context.DanhMucSanPhams
                //.Include(d => d.DanhMucCon)
                //.ThenInclude(d => d.DanhMucCon)
                .ToList();

            var danhMucViewModels = danhMuc.Select(d => new DanhMucViewModel
            {
                Id = d.DanhMucId,
                TenDanhMuc = d.TenDanhMuc,
                SoLuongSanPham = d.SanPhams?.Count ?? 0,
                DanhMucChaId = d.DanhMucChaId,
                DanhMucCon = d.DanhMucCon?.Select(cap2 => new DanhMucViewModel
                {
                    Id = cap2.DanhMucId,
                    TenDanhMuc = cap2.TenDanhMuc,
                    SoLuongSanPham = cap2.SanPhams?.Count ?? 0,
                    DanhMucChaId = cap2.DanhMucChaId,
                    DanhMucCon = cap2.DanhMucCon?.Select(cap3 => new DanhMucViewModel
                    {
                        Id = cap3.DanhMucId,
                        TenDanhMuc = cap3.TenDanhMuc,
                        SoLuongSanPham = cap3.SanPhams?.Count ?? 0,
                        DanhMucChaId = cap3.DanhMucChaId,
                    }).ToList()
                }).ToList()
            }).ToList();

            ViewBag.DanhMuc = danhMucViewModels;

            base.OnActionExecuting(context);
        }
    }
}
