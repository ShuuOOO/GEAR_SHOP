using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TL4_SHOP.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DanhMucSanPham",
                columns: table => new
                {
                    DanhMucID = table.Column<int>(type: "int", nullable: false),
                    TenDanhMuc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DanhMucChaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DanhMucS__1C53BA7B81C69B93", x => x.DanhMucID);
                    table.ForeignKey(
                        name: "FK_DanhMuc_ChaCon",
                        column: x => x.DanhMucChaId,
                        principalTable: "DanhMucSanPham",
                        principalColumn: "DanhMucID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DoanhThuTheoNam",
                columns: table => new
                {
                    Nam = table.Column<int>(type: "int", nullable: false),
                    TongSoDonHang = table.Column<int>(type: "int", nullable: true),
                    TongSoLuong = table.Column<int>(type: "int", nullable: true),
                    TongDoanhThu = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    LoiNhuan = table.Column<decimal>(type: "decimal(18,3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DoanhThu__C7D111C2525ECDB1", x => x.Nam);
                });

            migrationBuilder.CreateTable(
                name: "DoanhThuTheoNgay",
                columns: table => new
                {
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    TongSoDonHang = table.Column<int>(type: "int", nullable: true),
                    TongSoLuong = table.Column<int>(type: "int", nullable: true),
                    TongDoanhThu = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    LoiNhuan = table.Column<decimal>(type: "decimal(18,3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DoanhThu__6BCCE7B21837C0EC", x => x.Ngay);
                });

            migrationBuilder.CreateTable(
                name: "DoanhThuTheoThang",
                columns: table => new
                {
                    Nam = table.Column<int>(type: "int", nullable: false),
                    Thang = table.Column<int>(type: "int", nullable: false),
                    TongSoDonHang = table.Column<int>(type: "int", nullable: true),
                    TongSoLuong = table.Column<int>(type: "int", nullable: true),
                    TongDoanhThu = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    LoiNhuan = table.Column<decimal>(type: "decimal(18,3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DoanhThu__750C5E9697939130", x => new { x.Nam, x.Thang });
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    KhachHangID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KhachHan__880F211B510C2C99", x => x.KhachHangID);
                });

            migrationBuilder.CreateTable(
                name: "NhaCungCap",
                columns: table => new
                {
                    NhaCungCapID = table.Column<int>(type: "int", nullable: false),
                    TenNhaCungCap = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NhaCungC__8B8917276F4C76AB", x => x.NhaCungCapID);
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    NhanVienID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NhanVien__E27FD7EAB3185254", x => x.NhanVienID);
                });

            migrationBuilder.CreateTable(
                name: "TrangThaiDonHang",
                columns: table => new
                {
                    TrangThaiID = table.Column<int>(type: "int", nullable: false),
                    TenTrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TrangTha__D5BF1E850C628B11", x => x.TrangThaiID);
                });

            migrationBuilder.CreateTable(
                name: "DiaChi",
                columns: table => new
                {
                    DiaChiID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KhachHangID = table.Column<int>(type: "int", nullable: false),
                    TenNguoiNhan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoNha = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhuongXa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QuanHuyen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ThanhPho = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QuocGia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DiaChi__94E668E6FB309DBC", x => x.DiaChiID);
                    table.ForeignKey(
                        name: "FK__DiaChi__KhachHan__4222D4EF",
                        column: x => x.KhachHangID,
                        principalTable: "KhachHang",
                        principalColumn: "KhachHangID");
                });

            migrationBuilder.CreateTable(
                name: "SanPham",
                columns: table => new
                {
                    SanPhamID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenSanPham = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gia = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    SoLuongTon = table.Column<int>(type: "int", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DanhMucID = table.Column<int>(type: "int", nullable: true),
                    NhaCungCapID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SanPham__05180FF40BA47994", x => x.SanPhamID);
                    table.ForeignKey(
                        name: "FK__SanPham__DanhMuc__4CA06362",
                        column: x => x.DanhMucID,
                        principalTable: "DanhMucSanPham",
                        principalColumn: "DanhMucID");
                    table.ForeignKey(
                        name: "FK__SanPham__NhaCung__4D94879B",
                        column: x => x.NhaCungCapID,
                        principalTable: "NhaCungCap",
                        principalColumn: "NhaCungCapID");
                });

            migrationBuilder.CreateTable(
                name: "NhapHang",
                columns: table => new
                {
                    PhieuNhapID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NhaCungCapID = table.Column<int>(type: "int", nullable: true),
                    NgayNhap = table.Column<DateTime>(type: "datetime", nullable: false),
                    NhanVienID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NhapHang__DE3A388261A47F8B", x => x.PhieuNhapID);
                    table.ForeignKey(
                        name: "FK__NhapHang__NhaCun__48CFD27E",
                        column: x => x.NhaCungCapID,
                        principalTable: "NhaCungCap",
                        principalColumn: "NhaCungCapID");
                    table.ForeignKey(
                        name: "FK__NhapHang__NhanVi__49C3F6B7",
                        column: x => x.NhanVienID,
                        principalTable: "NhanVien",
                        principalColumn: "NhanVienID");
                });

            migrationBuilder.CreateTable(
                name: "TaoTaiKhoan",
                columns: table => new
                {
                    TaiKhoanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LoaiTaiKhoan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NhanVienID = table.Column<int>(type: "int", nullable: true),
                    KhachHangID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TaoTaiKh__9A124B658F6EA8E5", x => x.TaiKhoanID);
                    table.ForeignKey(
                        name: "FK__TaoTaiKho__Khach__3F466844",
                        column: x => x.KhachHangID,
                        principalTable: "KhachHang",
                        principalColumn: "KhachHangID");
                    table.ForeignKey(
                        name: "FK__TaoTaiKho__NhanV__3E52440B",
                        column: x => x.NhanVienID,
                        principalTable: "NhanVien",
                        principalColumn: "NhanVienID");
                });

            migrationBuilder.CreateTable(
                name: "DonHang",
                columns: table => new
                {
                    DonHangID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KhachHangID = table.Column<int>(type: "int", nullable: true),
                    NgayDatHang = table.Column<DateTime>(type: "datetime", nullable: false),
                    PhiVanChuyen = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DiaChiID = table.Column<int>(type: "int", nullable: true),
                    TrangThaiID = table.Column<int>(type: "int", nullable: false),
                    TenKhachHang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChiGiaoHang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhuongThucThanhToan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiDonHangText = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DonHang__D159F4DEB7BCC682", x => x.DonHangID);
                    table.ForeignKey(
                        name: "FK__DonHang__DiaChiI__5BE2A6F2",
                        column: x => x.DiaChiID,
                        principalTable: "DiaChi",
                        principalColumn: "DiaChiID");
                    table.ForeignKey(
                        name: "FK__DonHang__KhachHa__5AEE82B9",
                        column: x => x.KhachHangID,
                        principalTable: "KhachHang",
                        principalColumn: "KhachHangID");
                    table.ForeignKey(
                        name: "FK__DonHang__TrangTh__5CD6CB2B",
                        column: x => x.TrangThaiID,
                        principalTable: "TrangThaiDonHang",
                        principalColumn: "TrangThaiID");
                });

            migrationBuilder.CreateTable(
                name: "KhoHang",
                columns: table => new
                {
                    SanPhamID = table.Column<int>(type: "int", nullable: false),
                    SoLuongTon = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KhoHang__05180FF49CC24F5D", x => x.SanPhamID);
                    table.ForeignKey(
                        name: "FK__KhoHang__SanPham__5070F446",
                        column: x => x.SanPhamID,
                        principalTable: "SanPham",
                        principalColumn: "SanPhamID");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietNhapHang",
                columns: table => new
                {
                    ChiTietNhapHangID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhieuNhapID = table.Column<int>(type: "int", nullable: true),
                    SanPhamID = table.Column<int>(type: "int", nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGiaNhap = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietN__D9FE8A9035A9ABF3", x => x.ChiTietNhapHangID);
                    table.ForeignKey(
                        name: "FK__ChiTietNh__Phieu__6383C8BA",
                        column: x => x.PhieuNhapID,
                        principalTable: "NhapHang",
                        principalColumn: "PhieuNhapID");
                    table.ForeignKey(
                        name: "FK__ChiTietNh__SanPh__6477ECF3",
                        column: x => x.SanPhamID,
                        principalTable: "SanPham",
                        principalColumn: "SanPhamID");
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaiKhoanId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetToken_TaiKhoan",
                        column: x => x.TaiKhoanId,
                        principalTable: "TaoTaiKhoan",
                        principalColumn: "TaiKhoanID");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonHang",
                columns: table => new
                {
                    ChiTietID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonHangID = table.Column<int>(type: "int", nullable: false),
                    SanPhamID = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietD__B117E9EA30C44B1F", x => x.ChiTietID);
                    table.ForeignKey(
                        name: "FK__ChiTietDo__DonHa__5FB337D6",
                        column: x => x.DonHangID,
                        principalTable: "DonHang",
                        principalColumn: "DonHangID");
                    table.ForeignKey(
                        name: "FK__ChiTietDo__SanPh__60A75C0F",
                        column: x => x.SanPhamID,
                        principalTable: "SanPham",
                        principalColumn: "SanPhamID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_DonHangID",
                table: "ChiTietDonHang",
                column: "DonHangID");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_SanPhamID",
                table: "ChiTietDonHang",
                column: "SanPhamID");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietNhapHang_PhieuNhapID",
                table: "ChiTietNhapHang",
                column: "PhieuNhapID");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietNhapHang_SanPhamID",
                table: "ChiTietNhapHang",
                column: "SanPhamID");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucSanPham_DanhMucChaId",
                table: "DanhMucSanPham",
                column: "DanhMucChaId");

            migrationBuilder.CreateIndex(
                name: "IX_DiaChi_KhachHangID",
                table: "DiaChi",
                column: "KhachHangID");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_DiaChiID",
                table: "DonHang",
                column: "DiaChiID");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_KhachHangID",
                table: "DonHang",
                column: "KhachHangID");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_TrangThaiID",
                table: "DonHang",
                column: "TrangThaiID");

            migrationBuilder.CreateIndex(
                name: "IX_NhapHang_NhaCungCapID",
                table: "NhapHang",
                column: "NhaCungCapID");

            migrationBuilder.CreateIndex(
                name: "IX_NhapHang_NhanVienID",
                table: "NhapHang",
                column: "NhanVienID");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_TaiKhoanId",
                table: "PasswordResetTokens",
                column: "TaiKhoanId");

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_DanhMucID",
                table: "SanPham",
                column: "DanhMucID");

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_NhaCungCapID",
                table: "SanPham",
                column: "NhaCungCapID");

            migrationBuilder.CreateIndex(
                name: "IX_TaoTaiKhoan_KhachHangID",
                table: "TaoTaiKhoan",
                column: "KhachHangID");

            migrationBuilder.CreateIndex(
                name: "IX_TaoTaiKhoan_NhanVienID",
                table: "TaoTaiKhoan",
                column: "NhanVienID");

            migrationBuilder.CreateIndex(
                name: "UQ__TaoTaiKh__5C7E359EDB66121B",
                table: "TaoTaiKhoan",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__TaoTaiKh__A9D1053486347588",
                table: "TaoTaiKhoan",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietDonHang");

            migrationBuilder.DropTable(
                name: "ChiTietNhapHang");

            migrationBuilder.DropTable(
                name: "DoanhThuTheoNam");

            migrationBuilder.DropTable(
                name: "DoanhThuTheoNgay");

            migrationBuilder.DropTable(
                name: "DoanhThuTheoThang");

            migrationBuilder.DropTable(
                name: "KhoHang");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "DonHang");

            migrationBuilder.DropTable(
                name: "NhapHang");

            migrationBuilder.DropTable(
                name: "SanPham");

            migrationBuilder.DropTable(
                name: "TaoTaiKhoan");

            migrationBuilder.DropTable(
                name: "DiaChi");

            migrationBuilder.DropTable(
                name: "TrangThaiDonHang");

            migrationBuilder.DropTable(
                name: "DanhMucSanPham");

            migrationBuilder.DropTable(
                name: "NhaCungCap");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "KhachHang");
        }
    }
}
