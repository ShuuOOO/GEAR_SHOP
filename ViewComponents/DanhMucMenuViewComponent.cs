using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;
using TL4_SHOP.Models.ViewModels;

public class DanhMucMenuViewComponent : ViewComponent
{
    private readonly _4tlShopContext _context;

    public DanhMucMenuViewComponent(_4tlShopContext context)
    {
        _context = context;
    }

    public IViewComponentResult Invoke()
    {
        // Lấy tất cả danh mục và số lượng sản phẩm tương ứng
        var danhMucs = _context.DanhMucSanPhams
            .Include(d => d.SanPhams)
            .ToList();

        // Map sang ViewModel 3 cấp
        var danhMucVMs = danhMucs
            .Select(d => new DanhMucViewModel
            {
                Id = d.DanhMucId,
                TenDanhMuc = d.TenDanhMuc,
                DanhMucChaId = d.DanhMucChaId,
                SoLuongSanPham = d.SanPhams?.Count ?? 0
            })
            .ToList();

        // Gán cấp con (đệ quy thủ công 2 tầng)
        foreach (var cha in danhMucVMs.Where(d => d.DanhMucChaId == null))
        {
            cha.DanhMucCon = danhMucVMs
                .Where(c2 => c2.DanhMucChaId == cha.Id)
                .ToList();

            foreach (var cap2 in cha.DanhMucCon)
            {
                cap2.DanhMucCon = danhMucVMs
                    .Where(c3 => c3.DanhMucChaId == cap2.Id)
                    .ToList();
            }
        }

        var danhMucCha = danhMucVMs.Where(d => d.DanhMucChaId == null).ToList();

        return View(danhMucCha);
    }
}
