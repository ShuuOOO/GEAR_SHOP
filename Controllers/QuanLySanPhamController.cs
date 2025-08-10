using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using TL4_SHOP.Data;
using DataSanPham = TL4_SHOP.Data.SanPham;
using TL4_SHOP.Models.ViewModels;

namespace TL4_SHOP.Controllers
{
    [Authorize(Roles = "Nhân viên quản lý sản phẩm, Admin")]
    public class QuanLySanPhamController : Controller
    {
        private readonly _4tlShopContext _context;

        public QuanLySanPhamController(_4tlShopContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string searchTerm, int? categoryId, string stockStatus)
        {
            // Lấy danh sách sản phẩm
            var sanPhams = from s in _context.SanPhams.Include(s => s.DanhMuc).Include(s => s.NhaCungCap)
                           select s;

            // Áp dụng các bộ lọc
            if (!string.IsNullOrEmpty(searchTerm))
            {
                sanPhams = sanPhams.Where(s => s.TenSanPham.Contains(searchTerm));
                ViewBag.SearchTerm = searchTerm;
            }

            if (categoryId.HasValue)
            {
                sanPhams = sanPhams.Where(s => s.DanhMucId == categoryId);
                ViewBag.CategoryId = categoryId;
            }

            if (!string.IsNullOrEmpty(stockStatus))
            {
                switch (stockStatus)
                {
                    case "instock":
                        sanPhams = sanPhams.Where(s => s.SoLuongTon > 5);
                        break;
                    case "lowstock":
                        sanPhams = sanPhams.Where(s => s.SoLuongTon <= 5 && s.SoLuongTon > 0);
                        break;
                    case "outstock":
                        sanPhams = sanPhams.Where(s => s.SoLuongTon == 0);
                        break;
                }
                ViewBag.StockStatus = stockStatus;
            }

            // Tạo dropdown cho danh mục
            ViewBag.DanhMucs = new SelectList(_context.DanhMucSanPhams, "DanhMucId", "TenDanhMuc", categoryId);

            // Tạo dropdown cho trạng thái kho
            var stockStatusList = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Tất cả" },
            new SelectListItem { Value = "instock", Text = "Còn hàng" },
            new SelectListItem { Value = "lowstock", Text = "Sắp hết" },
            new SelectListItem { Value = "outstock", Text = "Hết hàng" }
        };
            ViewBag.StockStatusList = new SelectList(stockStatusList, "Value", "Text", stockStatus);

            return View(sanPhams.AsEnumerable());
        }

        // GET: AdminSanPham/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.DanhMuc)
                .Include(s => s.NhaCungCap)
                .FirstOrDefaultAsync(m => m.SanPhamId == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: AdminSanPham/Create
        public IActionResult Create()
        {
            ViewBag.DanhMucs = new SelectList(_context.DanhMucSanPhams, "DanhMucId", "TenDanhMuc");
            ViewBag.NhaCungCaps = new SelectList(_context.NhaCungCaps, "NhaCungCapId", "TenNhaCungCap");
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPhamViewModel model)
        {
            // Validation cơ bản trước
            if (string.IsNullOrWhiteSpace(model.TenSanPham))
            {
                ModelState.AddModelError("TenSanPham", "Tên sản phẩm không được để trống");
            }
            if (model.Gia < 0)
            {
                ModelState.AddModelError("Gia", "Giá không được âm");
            }
            if (model.SoLuongTon < 0)
            {
                ModelState.AddModelError("SoLuongTon", "Số lượng tồn không được âm");
            }

            // SỬA LỖI: Kiểm tra DanhMucId có tồn tại không - bỏ HasValue vì DanhMucId là int?
            if (model.DanhMucId != null && model.DanhMucId > 0)
            {
                var danhMucExists = await _context.DanhMucSanPhams.AnyAsync(d => d.DanhMucId == model.DanhMucId);
                if (!danhMucExists)
                {
                    ModelState.AddModelError("DanhMucId", "Danh mục không tồn tại");
                }
            }

            // SỬA LỖI: Kiểm tra NhaCungCapId - bỏ HasValue vì NhaCungCapId là int
            if (model.NhaCungCapId > 0)
            {
                var nhaCungCapExists = await _context.NhaCungCaps.AnyAsync(n => n.NhaCungCapId == model.NhaCungCapId);
                if (!nhaCungCapExists)
                {
                    ModelState.AddModelError("NhaCungCapId", "Nhà cung cấp không tồn tại");
                }
            }
            else
            {
                ModelState.AddModelError("NhaCungCapId", "Vui lòng chọn nhà cung cấp");
            }

            // Kiểm tra trùng tên sản phẩm
            var existingProduct = await _context.SanPhams
                .AnyAsync(s => s.TenSanPham.ToLower() == model.TenSanPham.ToLower());
            if (existingProduct)
            {
                ModelState.AddModelError("TenSanPham", "Tên sản phẩm đã tồn tại");
            }

            string savedImageFileName = null;

            try
            {
                // Xử lý upload ảnh trước khi validate ModelState
                if (model.HinhAnhFile != null)
                {
                    model.HinhAnh = await SaveImageAsync(model.HinhAnhFile); // Ví dụ: "Chuot_Faker_1.jpg"
                }
                else
                {
                    model.HinhAnh = "default-product.jpg";
                }


                if (ModelState.IsValid)
                {
                    var sanPham = new TL4_SHOP.Data.SanPham
                    {
                        TenSanPham = model.TenSanPham?.Trim(),
                        MoTa = model.MoTa?.Trim(),
                        Gia = model.Gia,
                        SoLuongTon = model.SoLuongTon,
                        HinhAnh = model.HinhAnh, // Lưu đúng tên file ở đây
                        DanhMucId = model.DanhMucId,
                        NhaCungCapId = model.NhaCungCapId
                    };

                    var strategy = _context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        _context.SanPhams.Add(sanPham);
                        await _context.SaveChangesAsync();
                    });

                    TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                    return RedirectToAction(nameof(Index));
                }



                if (ModelState.IsValid)
                {
                    // SỬA LỖI: Chỉ định rõ namespace để tránh ambiguous reference
                    var sanPham = new TL4_SHOP.Data.SanPham
                    {
                        TenSanPham = model.TenSanPham?.Trim(),
                        MoTa = model.MoTa?.Trim(),
                        Gia = model.Gia,
                        SoLuongTon = model.SoLuongTon,
                        HinhAnh = model.HinhAnh ?? "default-product.jpg", // Đặt ảnh mặc định nếu null
                        DanhMucId = model.DanhMucId,
                        NhaCungCapId = model.NhaCungCapId
                    };

                    // Sử dụng execution strategy để tránh lỗi transaction
                    var strategy = _context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        _context.SanPhams.Add(sanPham);
                        await _context.SaveChangesAsync();
                    });

                    TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (InvalidOperationException ex)
            {
                // Xóa ảnh đã upload nếu có lỗi
                if (!string.IsNullOrEmpty(savedImageFileName))
                {
                    DeleteUploadedImage(savedImageFileName);
                }
                ModelState.AddModelError("HinhAnhFile", ex.Message);
            }
            catch (DbUpdateException ex)
            {
                // Xóa ảnh đã upload nếu có lỗi
                if (!string.IsNullOrEmpty(savedImageFileName))
                {
                    DeleteUploadedImage(savedImageFileName);
                }

                // Xử lý lỗi database cụ thể
                var innerException = ex.InnerException?.Message ?? ex.Message;

                if (innerException.Contains("FOREIGN KEY"))
                {
                    ModelState.AddModelError("", "Lỗi khóa ngoại: Danh mục hoặc nhà cung cấp không hợp lệ");
                }
                else if (innerException.Contains("UNIQUE"))
                {
                    ModelState.AddModelError("TenSanPham", "Tên sản phẩm đã tồn tại");
                }
                else
                {
                    ModelState.AddModelError("", $"Lỗi cơ sở dữ liệu: {innerException}");
                }
            }
            catch (Exception ex)
            {
                // Xóa ảnh đã upload nếu có lỗi
                if (!string.IsNullOrEmpty(savedImageFileName))
                {
                    DeleteUploadedImage(savedImageFileName);
                }

                ModelState.AddModelError("", $"Có lỗi xảy ra khi thêm sản phẩm: {ex.Message}");

                // Log chi tiết lỗi để debug
                Console.WriteLine($"Error creating product: {ex}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException}");
                }
            }

            // Load lại dropdown khi lỗi
            ViewBag.DanhMucs = new SelectList(_context.DanhMucSanPhams, "DanhMucId", "TenDanhMuc", model.DanhMucId);
            ViewBag.NhaCungCaps = new SelectList(_context.NhaCungCaps, "NhaCungCapId", "TenNhaCungCap", model.NhaCungCapId);
            return View(model);
        }

        // Thêm method helper để xóa ảnh khi có lỗi
        private void DeleteUploadedImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            try
            {
                var imagePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot", "images", "products",
                    fileName);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Không thể xóa ảnh: {ex.Message}");
            }
        }

        // GET: AdminSanPham/Edit/5 - SỬA ĐỔI: Trả về SanPhamViewModel
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }

            // Chuyển đổi từ Entity sang ViewModel
            var model = new SanPhamViewModel
            {
                SanPhamId = sanPham.SanPhamId,
                TenSanPham = sanPham.TenSanPham,
                MoTa = sanPham.MoTa,
                Gia = sanPham.Gia,
                SoLuongTon = sanPham.SoLuongTon,
                HinhAnh = sanPham.HinhAnh,
                DanhMucId = sanPham.DanhMucId,
                NhaCungCapId = sanPham.NhaCungCapId
            };

            ViewBag.DanhMucs = new SelectList(_context.DanhMucSanPhams, "DanhMucId", "TenDanhMuc", sanPham.DanhMucId);
            ViewBag.NhaCungCaps = new SelectList(_context.NhaCungCaps, "NhaCungCapId", "TenNhaCungCap", sanPham.NhaCungCapId);
            return View(model);
        }

        // POST: AdminSanPham/Edit/5 - SỬA ĐỔI: Sử dụng SanPhamViewModel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SanPhamViewModel model)
        {
            if (id != model.SanPhamId)
                return NotFound();

            var existingSanPham = await _context.SanPhams.FindAsync(id);
            if (existingSanPham == null)
                return NotFound();

            // Validation cơ bản
            if (string.IsNullOrWhiteSpace(model.TenSanPham))
            {
                ModelState.AddModelError("TenSanPham", "Tên sản phẩm không được để trống");
            }
            if (model.Gia < 0)
            {
                ModelState.AddModelError("Gia", "Giá không được âm");
            }
            if (model.SoLuongTon < 0)
            {
                ModelState.AddModelError("SoLuongTon", "Số lượng tồn không được âm");
            }

            // Kiểm tra trùng tên sản phẩm (trừ chính nó)
            var existingProduct = await _context.SanPhams
                .AnyAsync(s => s.TenSanPham.ToLower() == model.TenSanPham.ToLower() && s.SanPhamId != model.SanPhamId);
            if (existingProduct)
            {
                ModelState.AddModelError("TenSanPham", "Tên sản phẩm đã tồn tại");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.DanhMucs = new SelectList(_context.DanhMucSanPhams, "DanhMucId", "TenDanhMuc", model.DanhMucId);
                ViewBag.NhaCungCaps = new SelectList(_context.NhaCungCaps, "NhaCungCapId", "TenNhaCungCap", model.NhaCungCapId);
                return View(model);
            }

            // Sử dụng execution strategy thay vì transaction thủ công
            var strategy = _context.Database.CreateExecutionStrategy();
            string newImageFileName = null;
            try
            {
                await strategy.ExecuteAsync(async () =>
                {
                    // Xử lý upload ảnh nếu có
                    if (model.HinhAnhFile != null)
                    {
                        newImageFileName = await SaveImageAsync(model.HinhAnhFile);

                        if (model.HinhAnhFile != null)
                        {
                            model.HinhAnh = await SaveImageAsync(model.HinhAnhFile); // Ví dụ: "Chuot_Faker_1.jpg"
                        }
                        else
                        {
                            model.HinhAnh = "default-product.jpg";
                        }
                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(existingSanPham.HinhAnh))
                        {
                            try
                            {
                                var oldImagePath = Path.Combine(
                                    Directory.GetCurrentDirectory(),
                                    "wwwroot", "images", "products",
                                    existingSanPham.HinhAnh);

                                if (System.IO.File.Exists(oldImagePath))
                                {
                                    System.IO.File.Delete(oldImagePath);
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log lỗi nhưng không dừng quá trình cập nhật
                                Console.WriteLine($"Không thể xóa ảnh cũ: {ex.Message}");
                            }
                        }

                        model.HinhAnh = newImageFileName;
                    }
                    else
                    {
                        // Giữ nguyên ảnh cũ nếu không upload ảnh mới
                        model.HinhAnh = existingSanPham.HinhAnh;
                    }

                    // Cập nhật thông tin sản phẩm
                    existingSanPham.TenSanPham = model.TenSanPham;
                    existingSanPham.MoTa = model.MoTa;
                    existingSanPham.Gia = model.Gia;
                    existingSanPham.HinhAnh = model.HinhAnh;
                    existingSanPham.DanhMucId = model.DanhMucId;
                    existingSanPham.NhaCungCapId = model.NhaCungCapId;

                    _context.Update(existingSanPham);
                    await _context.SaveChangesAsync();
                });

                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }

            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("HinhAnhFile", ex.Message);
            }
            catch (Exception ex)
            {
                // Xóa ảnh mới nếu đã upload nhưng lỗi khi lưu database
                if (!string.IsNullOrEmpty(newImageFileName))
                {
                    try
                    {
                        var newImagePath = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot", "images", "products",
                            newImageFileName);

                        if (System.IO.File.Exists(newImagePath))
                        {
                            System.IO.File.Delete(newImagePath);
                        }
                    }
                    catch { }
                }

                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật sản phẩm: " + ex.Message);
            }

            ViewBag.DanhMucs = new SelectList(_context.DanhMucSanPhams, "DanhMucId", "TenDanhMuc", model.DanhMucId);
            ViewBag.NhaCungCaps = new SelectList(_context.NhaCungCaps, "NhaCungCapId", "TenNhaCungCap", model.NhaCungCapId);
            return View(model);
        }
        // GET: AdminSanPham/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.DanhMuc)
                .Include(s => s.NhaCungCap)
                .FirstOrDefaultAsync(m => m.SanPhamId == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // POST: AdminSanPham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                // Xóa ảnh nếu có
                if (!string.IsNullOrEmpty(sanPham.HinhAnh))
                {
                    try
                    {
                        var imagePath = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot", "images", "products",
                            sanPham.HinhAnh);

                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log lỗi nhưng vẫn tiếp tục xóa sản phẩm
                        Console.WriteLine($"Không thể xóa ảnh: {ex.Message}");
                    }
                }

                _context.SanPhams.Remove(sanPham);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        // API endpoint cho tìm kiếm AJAX
        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 2)
            {
                return Json(new List<object>());
            }

            var suggestions = await _context.SanPhams
                .Where(s => s.TenSanPham.Contains(term))
                .Select(s => new
                {
                    s.SanPhamId,
                    s.TenSanPham,
                    s.Gia,
                    s.HinhAnh
                })
                .Take(10)
                .ToListAsync();

            return Json(suggestions);
        }

        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.SanPhamId == id);
        }

        // Phương thức helper để lưu ảnh
        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new InvalidOperationException("Không có file ảnh được chọn");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                throw new InvalidOperationException("Chỉ chấp nhận file ảnh có định dạng: " + string.Join(", ", allowedExtensions));

            if (imageFile.Length > 5 * 1024 * 1024)
                throw new InvalidOperationException("Kích thước file không được vượt quá 5MB");

            var originalFileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                originalFileName = originalFileName.Replace(c, '_');
            }

            var fileName = originalFileName + fileExtension;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            int count = 1;
            while (System.IO.File.Exists(Path.Combine(uploadsFolder, fileName)))
            {
                fileName = $"{originalFileName}_{count}{fileExtension}";
                count++;
            }

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return fileName; // Chỉ trả về tên file
        }



        // API endpoint để xuất Excel
        [HttpGet]
        public async Task<IActionResult> ExportToExcel()
        {
            try
            {
                var sanPhams = await _context.SanPhams
                    .Include(s => s.DanhMuc)
                    .Include(s => s.NhaCungCap)
                    .ToListAsync();

                var csv = new StringBuilder();
                csv.AppendLine("ID,Tên sản phẩm,Mô tả,Giá,Số lượng tồn,Danh mục,Nhà cung cấp,Hình ảnh");

                foreach (var item in sanPhams)
                {
                    csv.AppendLine($"{item.SanPhamId}," +
                                  $"\"{item.TenSanPham}\"," +
                                  $"\"{item.MoTa?.Replace("\"", "\"\"")}\"," +
                                  $"{item.Gia}," +
                                  $"{item.SoLuongTon}," +
                                  $"\"{item.DanhMuc?.TenDanhMuc}\"," +
                                  $"\"{item.NhaCungCap?.TenNhaCungCap}\"," +
                                  $"\"{item.HinhAnh}\"");
                }

                var bytes = Encoding.UTF8.GetBytes(csv.ToString());
                var fileName = $"DanhSachSanPham_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xuất file: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // debug create
        [HttpGet]
        public async Task<IActionResult> DebugCreate()
        {
            try
            {
                var danhMucs = await _context.DanhMucSanPhams.ToListAsync();
                var nhaCungCaps = await _context.NhaCungCaps.ToListAsync();

                return Json(new
                {
                    DanhMucs = danhMucs.Select(d => new { d.DanhMucId, d.TenDanhMuc }),
                    NhaCungCaps = nhaCungCaps.Select(n => new { n.NhaCungCapId, n.TenNhaCungCap }),
                    Message = "Debug thành công"
                });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message, InnerError = ex.InnerException?.Message });
            }
        }
    }
}
