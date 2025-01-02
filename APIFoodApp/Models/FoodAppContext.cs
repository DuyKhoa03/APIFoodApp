using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace APIFoodApp.Models;

public partial class FoodAppContext : DbContext
{
    public FoodAppContext()
    {
    }

    public FoodAppContext(DbContextOptions<FoodAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<ChiTietKhuyenMai> ChiTietKhuyenMais { get; set; }

    public virtual DbSet<DanhGium> DanhGia { get; set; }

    public virtual DbSet<DiaChi> DiaChis { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }

    public virtual DbSet<LichSuDonHang> LichSuDonHangs { get; set; }

    public virtual DbSet<LoaiKhuyenMai> LoaiKhuyenMais { get; set; }

    public virtual DbSet<LoaiSanPham> LoaiSanPhams { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }

    public virtual DbSet<PhuongThucThanhToan> PhuongThucThanhToans { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<ThongBao> ThongBaos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=TANDUNG;Database=FoodApp;Trusted_Connection=True;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => new { e.MaSanPham, e.MaDonHang }).HasName("PK__ChiTietD__3BEE1C6752D32057");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.GhiChu).HasColumnType("ntext");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDonHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__MaDon__5CD6CB2B");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__MaSan__60A75C0F");
        });

        modelBuilder.Entity<ChiTietKhuyenMai>(entity =>
        {
            entity.HasKey(e => new { e.MaSanPham, e.MaKhuyenMai }).HasName("PK__ChiTietK__3C322F1632D84D55");

            entity.ToTable("ChiTietKhuyenMai");

            entity.HasOne(d => d.MaKhuyenMaiNavigation).WithMany(p => p.ChiTietKhuyenMais)
                .HasForeignKey(d => d.MaKhuyenMai)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietKh__MaKhu__5AEE82B9");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChiTietKhuyenMais)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietKh__MaSan__619B8048");
        });

        modelBuilder.Entity<DanhGium>(entity =>
        {
            entity.HasKey(e => e.MaDanhGia).HasName("PK__DanhGia__AA9515BF046DE740");

            entity.Property(e => e.Anh).HasMaxLength(255);
            entity.Property(e => e.NoiDung).HasColumnType("ntext");
            entity.Property(e => e.ThoiGianCapNhat).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianDanhGia).HasColumnType("datetime");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGia__MaNguoi__5629CD9C");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGia__MaSanPh__5FB337D6");
        });

        modelBuilder.Entity<DiaChi>(entity =>
        {
            entity.HasKey(e => e.MaDiaChi).HasName("PK__DiaChi__EB61213EF3946D84");

            entity.ToTable("DiaChi");

            entity.Property(e => e.DiaChi1)
                .HasMaxLength(255)
                .HasColumnName("DiaChi");
            entity.Property(e => e.Ten).HasMaxLength(255);

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.DiaChis)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DiaChi__MaNguoiD__5535A963");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDonHang).HasName("PK__DonHang__129584ADE3DCE302");

            entity.ToTable("DonHang");

            entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.MaDiaChiNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaDiaChi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonHang__MaDiaCh__59FA5E80");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonHang__MaNguoi__571DF1D5");

            entity.HasOne(d => d.MaPhuongThucNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaPhuongThuc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonHang__MaPhuon__5DCAEF64");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.MaGioHang).HasName("PK__GioHang__F5001DA3DE7D86DD");

            entity.ToTable("GioHang");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHang__MaNguoi__59063A47");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHang__MaSanPh__628FA481");
        });

        modelBuilder.Entity<KhuyenMai>(entity =>
        {
            entity.HasKey(e => e.MaKhuyenMai).HasName("PK__KhuyenMa__6F56B3BDA8EAD9F0");

            entity.ToTable("KhuyenMai");

            entity.Property(e => e.BatDau).HasColumnType("datetime");
            entity.Property(e => e.DieuKienApDung).HasColumnType("text");
            entity.Property(e => e.GiaTri).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.KetThuc).HasColumnType("datetime");
            entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.Ten).HasMaxLength(100);

            entity.HasOne(d => d.MaLoaiNavigation).WithMany(p => p.KhuyenMais)
                .HasForeignKey(d => d.MaLoai)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhuyenMai__MaLoa__5EBF139D");
        });

        modelBuilder.Entity<LichSuDonHang>(entity =>
        {
            entity.HasKey(e => e.MaLichSu).HasName("PK__LichSuDo__C443222A8CEEF2D8");

            entity.ToTable("LichSuDonHang");

            entity.Property(e => e.GhiChu).HasColumnType("ntext");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.LichSuDonHangs)
                .HasForeignKey(d => d.MaDonHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LichSuDon__MaDon__5BE2A6F2");
        });

        modelBuilder.Entity<LoaiKhuyenMai>(entity =>
        {
            entity.HasKey(e => e.MaLoai).HasName("PK__LoaiKhuy__730A57591547088A");

            entity.ToTable("LoaiKhuyenMai");

            entity.Property(e => e.MoTa).HasColumnType("ntext");
            entity.Property(e => e.TenLoai).HasMaxLength(100);
        });

        modelBuilder.Entity<LoaiSanPham>(entity =>
        {
            entity.HasKey(e => e.MaLoai).HasName("PK__LoaiSanP__730A5759B2588C9B");

            entity.ToTable("LoaiSanPham");

            entity.Property(e => e.MoTa).HasColumnType("ntext");
            entity.Property(e => e.TenLoai).HasMaxLength(100);
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__C539D76208344482");

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.SoDienThoai, "UQ__NguoiDun__0389B7BD34EF47B5").IsUnique();

            entity.HasIndex(e => e.TenDangNhap, "UQ__NguoiDun__55F68FC0183A715E").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__NguoiDun__A9D1053452717184").IsUnique();

            entity.Property(e => e.Anh).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");
            entity.Property(e => e.NgayDangKy).HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TenNguoiDung).HasMaxLength(100);
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.MaNhaCungCap).HasName("PK__NhaCungC__53DA9205DC4A1224");

            entity.ToTable("NhaCungCap");

            entity.Property(e => e.DiaChi).HasMaxLength(100);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenNhaCungCap).HasMaxLength(100);
        });

        modelBuilder.Entity<PhuongThucThanhToan>(entity =>
        {
            entity.HasKey(e => e.MaPhuongThuc).HasName("PK__PhuongTh__35F7404E06F8C6A7");

            entity.ToTable("PhuongThucThanhToan");

            entity.Property(e => e.MoTa).HasColumnType("ntext");
            entity.Property(e => e.Ten).HasMaxLength(100);
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSanPham).HasName("PK__SanPham__FAC7442D12B352F8");

            entity.ToTable("SanPham");

            entity.Property(e => e.Anh1).HasMaxLength(255);
            entity.Property(e => e.Anh2).HasMaxLength(255);
            entity.Property(e => e.Anh3).HasMaxLength(255);
            entity.Property(e => e.Anh4).HasMaxLength(255);
            entity.Property(e => e.Anh5).HasMaxLength(255);
            entity.Property(e => e.Gia).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MoTa).HasColumnType("ntext");
            entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.TenSanPham).HasMaxLength(100);

            entity.HasOne(d => d.MaLoaiNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaLoai)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaLoai__6383C8BA");

            entity.HasOne(d => d.MaNhaCungCapNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaNhaCungCap)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaNhaCu__6477ECF3");
        });

        modelBuilder.Entity<ThongBao>(entity =>
        {
            entity.HasKey(e => e.MaThongBao).HasName("PK__ThongBao__04DEB54EB29F7AB7");

            entity.ToTable("ThongBao");

            entity.Property(e => e.DieuKienKichHoat).HasMaxLength(255);
            entity.Property(e => e.MoTa).HasColumnType("ntext");
            entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.NoiDung).HasColumnType("ntext");
            entity.Property(e => e.Ten).HasMaxLength(100);
            entity.Property(e => e.TheLoai).HasMaxLength(100);

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.ThongBaos)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ThongBao__MaNguo__5812160E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
