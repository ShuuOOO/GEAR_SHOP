using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TL4_SHOP.Models;
using System.IO;
using TL4_SHOP.Data;

public static class InvoiceGenerator
{
    public static byte[] CreateInvoice(DonHang donHang)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);

                page.Header().Text($"HÓA ĐƠN MUA HÀNG # {donHang.DonHangId}")
                             .FontSize(20).Bold().FontColor(Colors.Blue.Medium);

                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Text($"Khách hàng: {donHang.TenKhachHang}");
                    col.Item().Text($"SĐT: {donHang.SoDienThoai}");
                    col.Item().Text($"Địa chỉ: {donHang.DiaChiGiaoHang}");
                    col.Item().Text($"Ngày đặt: {donHang.NgayDatHang:dd/MM/yyyy HH:mm}");

                    col.Item().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Sản phẩm").Bold();
                            header.Cell().Text("SL").Bold();
                            header.Cell().Text("Đơn giá").Bold();
                            header.Cell().Text("Thành tiền").Bold();
                        });

                        foreach (var item in donHang.ChiTietDonHangs)
                        {
                            table.Cell().Text(item.SanPham?.TenSanPham ?? "Sản phẩm?");
                            table.Cell().Text(item.SoLuong.ToString());
                            table.Cell().Text(item.DonGia.ToString("N0") + " đ");
                            table.Cell().Text(item.ThanhTien.ToString("N0") + " đ");
                        }
                    });

                    col.Item().PaddingTop(10).AlignRight().Text($"Tổng tiền: {donHang.TongTien:N0} đ").Bold();
                });

                page.Footer().AlignCenter().Text("Cảm ơn bạn đã mua hàng tại 4TL_SHOP!");
            });
        }).GeneratePdf();
    }
}
