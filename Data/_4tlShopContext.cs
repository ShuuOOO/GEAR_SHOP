using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Models;

namespace TL4_SHOP.Data;

public partial class _4tlShopContext : DbContext
{
    public _4tlShopContext()
    {
    }

    public _4tlShopContext(DbContextOptions<_4tlShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<ChiTietNhapHang> ChiTietNhapHangs { get; set; }

    public virtual DbSet<DanhMucSanPham> DanhMucSanPhams { get; set; }

    public virtual DbSet<DiaChi> DiaChis { get; set; }

    public virtual DbSet<DoanhThuTheoNam> DoanhThuTheoNams { get; set; }

    public virtual DbSet<DoanhThuTheoNgay> DoanhThuTheoNgays { get; set; }

    public virtual DbSet<DoanhThuTheoThang> DoanhThuTheoThangs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<KhoHang> KhoHangs { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<NhapHang> NhapHangs { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<TaoTaiKhoan> TaoTaiKhoans { get; set; }

    public virtual DbSet<TrangThaiDonHang> TrangThaiDonHangs { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }



    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Data Source=LENHATTAN\\SQLEXPRESS;Initial Catalog=4TL_SHOP;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.ChiTietId).HasName("PK__ChiTietD__B117E9EA30C44B1F");

            entity.ToTable("ChiTietDonHang", tb =>
                {
                    tb.HasTrigger("trg_CTDH_InsertUpdate");
                    tb.HasTrigger("trg_CapNhatSoLuongTon_ChiTietDonHang");
                    tb.HasTrigger("trg_DonHang_UpdateTongTien");
                });

            entity.Property(e => e.ChiTietId).HasColumnName("ChiTietID");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.DonHangId).HasColumnName("DonHangID");
            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 3)");

            entity.HasOne(d => d.DonHang).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.DonHangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__DonHa__5FB337D6");

            entity.HasOne(d => d.SanPham).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.SanPhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__SanPh__60A75C0F");
        });

        modelBuilder.Entity<ChiTietNhapHang>(entity =>
        {
            entity.HasKey(e => e.ChiTietNhapHangId).HasName("PK__ChiTietN__D9FE8A9035A9ABF3");

            entity.ToTable("ChiTietNhapHang", tb =>
                {
                    tb.HasTrigger("trg_CapNhatSoLuongTonKhiNhap");
                    tb.HasTrigger("trg_TinhTongTien");
                });

            entity.Property(e => e.ChiTietNhapHangId).HasColumnName("ChiTietNhapHangID");
            entity.Property(e => e.DonGiaNhap).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.PhieuNhapId).HasColumnName("PhieuNhapID");
            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 3)");

            entity.HasOne(d => d.PhieuNhap).WithMany(p => p.ChiTietNhapHangs)
                .HasForeignKey(d => d.PhieuNhapId)
                .HasConstraintName("FK__ChiTietNh__Phieu__6383C8BA");

            entity.HasOne(d => d.SanPham).WithMany(p => p.ChiTietNhapHangs)
                .HasForeignKey(d => d.SanPhamId)
                .HasConstraintName("FK__ChiTietNh__SanPh__6477ECF3");
        });

        modelBuilder.Entity<DanhMucSanPham>(entity =>
        {
            entity.HasKey(e => e.DanhMucId).HasName("PK__DanhMucS__1C53BA7B81C69B93");

            entity.ToTable("DanhMucSanPham");

            entity.Property(e => e.DanhMucId)
                .ValueGeneratedNever()
                .HasColumnName("DanhMucID");

            entity.Property(e => e.TenDanhMuc).HasMaxLength(100);

            entity.Property(e => e.MoTa).IsRequired();

            entity.Property(e => e.DanhMucChaId).HasColumnName("DanhMucChaId"); 

            // Quan hệ cha – con
            entity.HasOne(d => d.DanhMucCha)
                .WithMany(p => p.DanhMucCon)
                .HasForeignKey(d => d.DanhMucChaId)
                .HasConstraintName("FK_DanhMuc_ChaCon")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DiaChi>(entity =>
        {
            entity.HasKey(e => e.DiaChiId).HasName("PK__DiaChi__94E668E6FB309DBC");

            entity.ToTable("DiaChi", tb => tb.HasTrigger("trg_AutoFill_DiaChi"));

            entity.Property(e => e.DiaChiId).HasColumnName("DiaChiID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.PhuongXa).HasMaxLength(100);
            entity.Property(e => e.QuanHuyen).HasMaxLength(100);
            entity.Property(e => e.QuocGia).HasMaxLength(100);
            entity.Property(e => e.SoNha).HasMaxLength(255);
            entity.Property(e => e.TenNguoiNhan).HasMaxLength(100);
            entity.Property(e => e.ThanhPho).HasMaxLength(100);
            entity.Property(e => e.ZipCode).HasMaxLength(20);

            entity.HasOne(d => d.KhachHang).WithMany(p => p.DiaChis)
                .HasForeignKey(d => d.KhachHangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DiaChi__KhachHan__4222D4EF");
        });

        modelBuilder.Entity<DoanhThuTheoNam>(entity =>
        {
            entity.HasKey(e => e.Nam).HasName("PK__DoanhThu__C7D111C2525ECDB1");

            entity.ToTable("DoanhThuTheoNam");

            entity.Property(e => e.Nam).ValueGeneratedNever();
            entity.Property(e => e.LoiNhuan).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.TongDoanhThu).HasColumnType("decimal(18, 3)");
        });

        modelBuilder.Entity<DoanhThuTheoNgay>(entity =>
        {
            entity.HasKey(e => e.Ngay).HasName("PK__DoanhThu__6BCCE7B21837C0EC");

            entity.ToTable("DoanhThuTheoNgay", tb =>
                {
                    tb.HasTrigger("trg_CapNhatDoanhThuTheoNam");
                    tb.HasTrigger("trg_CapNhatDoanhThuTheoThang");
                });

            entity.Property(e => e.LoiNhuan).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.TongDoanhThu).HasColumnType("decimal(18, 3)");
        });

        modelBuilder.Entity<DoanhThuTheoThang>(entity =>
        {
            entity.HasKey(e => new { e.Nam, e.Thang }).HasName("PK__DoanhThu__750C5E9697939130");

            entity.ToTable("DoanhThuTheoThang");

            entity.Property(e => e.LoiNhuan).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.TongDoanhThu).HasColumnType("decimal(18, 3)");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.DonHangId).HasName("PK__DonHang__D159F4DEB7BCC682");

            entity.ToTable("DonHang", tb => tb.HasTrigger("trg_CapNhatDoanhThuTheoNgay"));

            entity.Property(e => e.DonHangId).HasColumnName("DonHangID");
            entity.Property(e => e.DiaChiId).HasColumnName("DiaChiID");
            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.NgayDatHang).HasColumnType("datetime");
            entity.Property(e => e.PhiVanChuyen).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.TrangThaiId).HasColumnName("TrangThaiID");

            entity.HasOne(d => d.DiaChi).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.DiaChiId)
                .HasConstraintName("FK__DonHang__DiaChiI__5BE2A6F2");

            entity.HasOne(d => d.KhachHang).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.KhachHangId)
                .HasConstraintName("FK__DonHang__KhachHa__5AEE82B9");

            entity.HasOne(d => d.TrangThai).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.TrangThaiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonHang__TrangTh__5CD6CB2B");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.KhachHangId).HasName("PK__KhachHan__880F211B510C2C99");

            entity.ToTable("KhachHang");

            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<KhoHang>(entity =>
        {
            entity.HasKey(e => e.SanPhamId).HasName("PK__KhoHang__05180FF49CC24F5D");

            entity.ToTable("KhoHang");

            entity.Property(e => e.SanPhamId)
                .ValueGeneratedNever()
                .HasColumnName("SanPhamID");

            entity.HasOne(d => d.SanPham).WithOne(p => p.KhoHang)
                .HasForeignKey<KhoHang>(d => d.SanPhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhoHang__SanPham__5070F446");
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.NhaCungCapId).HasName("PK__NhaCungC__8B8917276F4C76AB");

            entity.ToTable("NhaCungCap");

            entity.Property(e => e.NhaCungCapId)
                .ValueGeneratedNever()
                .HasColumnName("NhaCungCapID");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.TenNhaCungCap).HasMaxLength(100);
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.NhanVienId).HasName("PK__NhanVien__E27FD7EAB3185254");

            entity.ToTable("NhanVien");

            entity.Property(e => e.NhanVienId).HasColumnName("NhanVienID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.VaiTro).HasMaxLength(50);
        });

        modelBuilder.Entity<NhapHang>(entity =>
        {
            entity.HasKey(e => e.PhieuNhapId).HasName("PK__NhapHang__DE3A388261A47F8B");

            entity.ToTable("NhapHang");

            entity.Property(e => e.PhieuNhapId).HasColumnName("PhieuNhapID");
            entity.Property(e => e.NgayNhap).HasColumnType("datetime");
            entity.Property(e => e.NhaCungCapId).HasColumnName("NhaCungCapID");
            entity.Property(e => e.NhanVienId).HasColumnName("NhanVienID");

            entity.HasOne(d => d.NhaCungCap).WithMany(p => p.NhapHangs)
                .HasForeignKey(d => d.NhaCungCapId)
                .HasConstraintName("FK__NhapHang__NhaCun__48CFD27E");

            entity.HasOne(d => d.NhanVien).WithMany(p => p.NhapHangs)
                .HasForeignKey(d => d.NhanVienId)
                .HasConstraintName("FK__NhapHang__NhanVi__49C3F6B7");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.SanPhamId).HasName("PK__SanPham__05180FF40BA47994");

            entity.ToTable("SanPham", tb => tb.HasTrigger("trg_InsertKhoHang"));

            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");
            entity.Property(e => e.DanhMucId).HasColumnName("DanhMucID");
            entity.Property(e => e.Gia).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.HinhAnh).HasMaxLength(255);
            entity.Property(e => e.NhaCungCapId).HasColumnName("NhaCungCapID");
            entity.Property(e => e.TenSanPham).HasMaxLength(100);

            entity.HasOne(d => d.DanhMuc).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.DanhMucId)
                .HasConstraintName("FK__SanPham__DanhMuc__4CA06362");

            entity.HasOne(d => d.NhaCungCap).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.NhaCungCapId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__NhaCung__4D94879B");
        });

        modelBuilder.Entity<TaoTaiKhoan>(entity =>
        {
            entity.HasKey(e => e.TaiKhoanId).HasName("PK__TaoTaiKh__9A124B658F6EA8E5");

            entity.ToTable("TaoTaiKhoan", tb =>
                {
                    tb.HasTrigger("trg_InsertKhachHangFromTaiKhoan");
                    tb.HasTrigger("trg_InsertNhanVienFromTaiKhoan");
                    tb.HasTrigger("trg_UpdateIDs_AfterInsert");
                });

            entity.HasIndex(e => e.Phone, "UQ__TaoTaiKh__5C7E359EDB66121B").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__TaoTaiKh__A9D1053486347588").IsUnique();

            entity.Property(e => e.TaiKhoanId).HasColumnName("TaiKhoanID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.LoaiTaiKhoan).HasMaxLength(20);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.NhanVienId).HasColumnName("NhanVienID");
            entity.Property(e => e.Phone).HasMaxLength(20);

            entity.HasOne(d => d.KhachHang)
              .WithOne(p => p.TaiKhoan)
              .HasForeignKey<TaoTaiKhoan>(d => d.KhachHangId)
              .HasConstraintName("FK__TaoTaiKho__Khach__3F466844");
               

            entity.HasOne(d => d.NhanVien).WithMany(p => p.TaoTaiKhoans)
                .HasForeignKey(d => d.NhanVienId)
                .HasConstraintName("FK__TaoTaiKho__NhanV__3E52440B");
        });

        modelBuilder.Entity<TrangThaiDonHang>(entity =>
        {
            entity.HasKey(e => e.TrangThaiId).HasName("PK__TrangTha__D5BF1E850C628B11");

            entity.ToTable("TrangThaiDonHang");

            entity.Property(e => e.TrangThaiId)
                .ValueGeneratedNever()
                .HasColumnName("TrangThaiID");
            entity.Property(e => e.TenTrangThai).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.ExpiryDate)
                .IsRequired();

            entity.Property(e => e.IsUsed)
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.TaiKhoan)
                .WithMany()
                .HasForeignKey(d => d.TaiKhoanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PasswordResetToken_TaiKhoan");
        });
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
