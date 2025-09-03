using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TL4_SHOP.Data;

public partial class Tl4ShopContext : DbContext
{
    public Tl4ShopContext()
    {
    }

    public Tl4ShopContext(DbContextOptions<Tl4ShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<HangHoa> HangHoas { get; set; }

    public virtual DbSet<Loai> Loais { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=TL4_SHOP;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HangHoa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HangHoa__3213E83F6B2D0A50");

            entity.ToTable("HangHoa");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.GiaSanPham)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("giaSanPham");
            entity.Property(e => e.Ten)
                .HasMaxLength(100)
                .HasColumnName("ten");
        });

        modelBuilder.Entity<Loai>(entity =>
        {
            entity.HasKey(e => e.MaLoai).HasName("PK__Loai__730A57595B635254");

            entity.ToTable("Loai");

            entity.Property(e => e.MaLoai).ValueGeneratedNever();
            entity.Property(e => e.Hinh).HasMaxLength(50);
            entity.Property(e => e.MoTa).HasMaxLength(100);
            entity.Property(e => e.TenLoai).HasMaxLength(100);
            entity.Property(e => e.TenLoaiAlias).HasMaxLength(100);

            entity.HasOne(d => d.MaLoaiNavigation).WithOne(p => p.Loai)
                .HasForeignKey<Loai>(d => d.MaLoai)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Loai_HangHoa");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
