using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<TaoTaiKhoan> TaoTaiKhoans { get; set; }

    public virtual DbSet<TrangThaiDonHang> TrangThaiDonHangs { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    public virtual DbSet<WishlistItem> WishlistItems { get; set; }

    public virtual DbSet<TechNews> TechNews { get; set; } = null!;
    public virtual DbSet<ChatMessage> ChatMessages { get; set; } = null!;

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=4TL_SHOP;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.ChiTietId).HasName("PK__ChiTietD__B117E9EAF1299D72");

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
                .HasConstraintName("FK__ChiTietDo__DonHa__656C112C");

            entity.HasOne(d => d.SanPham).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.SanPhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__SanPh__66603565");
        });

        modelBuilder.Entity<ChiTietNhapHang>(entity =>
        {
            entity.HasKey(e => e.ChiTietNhapHangId).HasName("PK__ChiTietN__D9FE8A90D5D8DB80");

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
                .HasConstraintName("FK__ChiTietNh__Phieu__6754599E");

            entity.HasOne(d => d.SanPham).WithMany(p => p.ChiTietNhapHangs)
                .HasForeignKey(d => d.SanPhamId)
                .HasConstraintName("FK__ChiTietNh__SanPh__68487DD7");
        });

        modelBuilder.Entity<DanhMucSanPham>(entity =>
        {
            entity.HasKey(e => e.DanhMucId).HasName("PK__DanhMucS__1C53BA7B71EFB0D1");

            entity.ToTable("DanhMucSanPham");

            entity.Property(e => e.DanhMucId)
                .ValueGeneratedNever()
                .HasColumnName("DanhMucID");
            entity.Property(e => e.TenDanhMuc).HasMaxLength(100);

            entity.HasOne(d => d.DanhMucCha).WithMany(p => p.DanhMucCon)
                .HasForeignKey(d => d.DanhMucChaId)
                .HasConstraintName("FK_DanhMuc_ChaCon");
        });

        modelBuilder.Entity<DiaChi>(entity =>
        {
            entity.HasKey(e => e.DiaChiId).HasName("PK__DiaChi__94E668E696DC2723");

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
                .HasConstraintName("FK__DiaChi__KhachHan__6A30C649");
        });

        modelBuilder.Entity<DoanhThuTheoNam>(entity =>
        {
            entity.HasKey(e => e.Nam).HasName("PK__DoanhThu__C7D111C2C4C815AF");

            entity.ToTable("DoanhThuTheoNam");

            entity.Property(e => e.Nam).ValueGeneratedNever();
            entity.Property(e => e.LoiNhuan).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.TongDoanhThu).HasColumnType("decimal(18, 3)");
        });

        modelBuilder.Entity<DoanhThuTheoNgay>(entity =>
        {
            entity.HasKey(e => e.Ngay).HasName("PK__DoanhThu__6BCCE7B20FFD82FF");

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
            entity.HasKey(e => new { e.Nam, e.Thang }).HasName("PK__DoanhThu__750C5E96CD37571B");

            entity.ToTable("DoanhThuTheoThang");

            entity.Property(e => e.LoiNhuan).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.TongDoanhThu).HasColumnType("decimal(18, 3)");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.DonHangId).HasName("PK__DonHang__D159F4DE93253B15");

            entity.ToTable("DonHang", tb => tb.HasTrigger("trg_CapNhatDoanhThuTheoNgay"));

            entity.Property(e => e.DonHangId).HasColumnName("DonHangID");
            entity.Property(e => e.DiaChiGiaoHang).HasMaxLength(255);
            entity.Property(e => e.DiaChiId).HasColumnName("DiaChiID");
            entity.Property(e => e.EmailNguoiDat).HasMaxLength(255);
            entity.Property(e => e.TaiKhoanId).HasColumnName("KhachHangID");
            entity.Property(e => e.NgayDatHang).HasColumnType("datetime");
            entity.Property(e => e.PhiVanChuyen).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.PhuongThucThanhToan).HasMaxLength(100);
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TenKhachHang).HasMaxLength(100);
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.TrangThaiDonHangText).HasMaxLength(100);
            entity.Property(e => e.TrangThaiId).HasColumnName("TrangThaiID");

            entity.HasOne(d => d.DiaChi).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.DiaChiId)
                .HasConstraintName("FK__DonHang__DiaChiI__6B24EA82");

            entity.HasOne(d => d.KhachHang).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.TaiKhoanId)
                .HasConstraintName("FK__DonHang__KhachHa__6C190EBB");

            entity.HasOne(d => d.TrangThai).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.TrangThaiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonHang__TrangTh__6D0D32F4");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.KhachHangId).HasName("PK__KhachHan__880F211BBB2BF01B");

            entity.ToTable("KhachHang");

            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<KhoHang>(entity =>
        {
            entity.HasKey(e => e.SanPhamId).HasName("PK__KhoHang__05180FF4F3BF291A");

            entity.ToTable("KhoHang");

            entity.Property(e => e.SanPhamId)
                .ValueGeneratedNever()
                .HasColumnName("SanPhamID");

            entity.HasOne(d => d.SanPham).WithOne(p => p.KhoHang)
                .HasForeignKey<KhoHang>(d => d.SanPhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhoHang__SanPham__6E01572D");
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.NhaCungCapId).HasName("PK__NhaCungC__8B8917272738218A");

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
            entity.HasKey(e => e.NhanVienId).HasName("PK__NhanVien__E27FD7EA80CBD491");

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
            entity.HasKey(e => e.PhieuNhapId).HasName("PK__NhapHang__DE3A3882C632D946");

            entity.ToTable("NhapHang");

            entity.Property(e => e.PhieuNhapId).HasColumnName("PhieuNhapID");
            entity.Property(e => e.NgayNhap).HasColumnType("datetime");
            entity.Property(e => e.NhaCungCapId).HasColumnName("NhaCungCapID");
            entity.Property(e => e.NhanVienId).HasColumnName("NhanVienID");

            entity.HasOne(d => d.NhaCungCap).WithMany(p => p.NhapHangs)
                .HasForeignKey(d => d.NhaCungCapId)
                .HasConstraintName("FK__NhapHang__NhaCun__6EF57B66");

            entity.HasOne(d => d.NhanVien).WithMany(p => p.NhapHangs)
                .HasForeignKey(d => d.NhanVienId)
                .HasConstraintName("FK__NhapHang__NhanVi__6FE99F9F");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Password__3214EC07288ACA41");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.IsUsed).HasDefaultValue(false);
            entity.Property(e => e.Token).HasMaxLength(255);

            entity.HasOne(d => d.TaiKhoan).WithMany(p => p.PasswordResetTokens)
                .HasForeignKey(d => d.TaiKhoanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PasswordR__TaiKh__70DDC3D8");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.SanPhamId).HasName("PK__SanPham__05180FF4AC4878EE");

            entity.ToTable("SanPham", tb => tb.HasTrigger("trg_InsertKhoHang"));

            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");
            entity.Property(e => e.DanhMucId).HasColumnName("DanhMucID");
            entity.Property(e => e.Gia).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.GiaSauGiam).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.HinhAnh).HasMaxLength(255);
            entity.Property(e => e.LaNoiBat).HasDefaultValue(false);
            entity.Property(e => e.NhaCungCapId).HasColumnName("NhaCungCapID");
            entity.Property(e => e.TenSanPham).HasMaxLength(100);

            entity.HasOne(d => d.DanhMuc).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.DanhMucId)
                .HasConstraintName("FK__SanPham__DanhMuc__71D1E811");

            entity.HasOne(d => d.NhaCungCap).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.NhaCungCapId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__NhaCung__72C60C4A");
        });

        modelBuilder.Entity<TaoTaiKhoan>(entity =>
        {
            entity.HasKey(e => e.TaiKhoanId).HasName("PK__TaoTaiKh__9A124B658310C38B");

            entity.ToTable("TaoTaiKhoan", tb =>
            {
                tb.HasTrigger("trg_InsertKhachHangFromTaiKhoan");
                tb.HasTrigger("trg_InsertNhanVienFromTaiKhoan");
                tb.HasTrigger("trg_UpdateIDs_AfterInsert");
            });

            entity.HasIndex(e => e.Phone, "UQ__TaoTaiKh__5C7E359EF074C186").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__TaoTaiKh__A9D1053489221C10").IsUnique();

            entity.Property(e => e.TaiKhoanId).HasColumnName("TaiKhoanID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.LoaiTaiKhoan).HasMaxLength(20);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.NhanVienId).HasColumnName("NhanVienID");
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.VaiTro).HasMaxLength(100);

            entity.HasOne(d => d.KhachHang).WithMany(p => p.TaoTaiKhoans)
                .HasForeignKey(d => d.KhachHangId)
                .HasConstraintName("FK__TaoTaiKho__Khach__73BA3083");

            entity.HasOne(d => d.NhanVien).WithMany(p => p.TaoTaiKhoans)
                .HasForeignKey(d => d.NhanVienId)
                .HasConstraintName("FK__TaoTaiKho__NhanV__74AE54BC");
        });

        modelBuilder.Entity<TrangThaiDonHang>(entity =>
        {
            entity.HasKey(e => e.TrangThaiId).HasName("PK__TrangTha__D5BF1E85ABD8D594");

            entity.ToTable("TrangThaiDonHang");

            entity.Property(e => e.TrangThaiId)
                .ValueGeneratedNever()
                .HasColumnName("TrangThaiID");
            entity.Property(e => e.TenTrangThai).HasMaxLength(50);
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.WishlistId).HasName("PK__Wishlist__233189EBE961249E");

            entity.Property(e => e.SessionId).HasMaxLength(100);

            entity.HasOne(d => d.TaoTaiKhoan).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.TaiKhoanId)
                .HasConstraintName("FK__Wishlists__TaiKhoan__778AC167");
        });


        modelBuilder.Entity<WishlistItem>(entity =>
        {
            entity.HasKey(e => e.WishlistItemId).HasName("PK__Wishlist__171E21A136D407A9");

            entity.HasOne(d => d.SanPham).WithMany(p => p.WishlistItems)
                .HasForeignKey(d => d.SanPhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WishlistI__SanPh__75A278F5");

            entity.HasOne(d => d.Wishlist).WithMany(p => p.WishlistItems)
                .HasForeignKey(d => d.WishlistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WishlistI__Wishl__76969D2E");
            // Thêm mapping cho TaoTaiKhoan
            entity.HasOne(d => d.TaoTaiKhoan)
                 .WithMany(t => t.WishlistItems) // trùng với tên mới
                 .HasForeignKey(d => d.TaiKhoanId)
                 .HasConstraintName("FK_WishlistItem_TaoTaiKhoan");
        });

        modelBuilder.Entity<TechNews>(entity =>
        {
            entity.ToTable("TechNews");
            entity.HasKey(e => e.TechNewsId);
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Slug).HasMaxLength(220).IsRequired();
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.Summary).HasMaxLength(400);
            entity.Property(e => e.CoverImage).HasMaxLength(300);
            entity.Property(e => e.Author).HasMaxLength(100);
            entity.Property(e => e.Tags).HasMaxLength(200);
            entity.HasIndex(e => e.PublishedAt);
            entity.HasIndex(e => new { e.IsFeatured, e.PublishedAt });
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.ToTable("ChatMessages"); // tên bảng trong database

            entity.HasKey(e => e.Id); // khóa chính

            entity.Property(e => e.SenderId)
                .IsRequired(); // Id người gửi

            entity.Property(e => e.ReceiverId)
                .IsRequired(); // Id người nhận (TaoTaiKhoan.TaiKhoanId)

            entity.Property(e => e.SenderName)
                .IsRequired()
                .HasMaxLength(50); // "Admin" hoặc tên khách

            entity.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(1000); // Nội dung tin nhắn

            entity.Property(e => e.Timestamp)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()"); // ngày giờ gửi
        });



        // [THÊM] trong OnModelCreating
        modelBuilder.Entity<DanhMucSanPham>()
            .Property(p => p.DanhMucId)
            .ValueGeneratedOnAdd();


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}