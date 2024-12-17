using System;
using System.Collections.Generic;

namespace APIFoodApp.Models;

public partial class SanPham
{
    public int MaSanPham { get; set; }

    public int MaLoai { get; set; }

    public int MaNhaCungCap { get; set; }

    public string? TenSanPham { get; set; }

    public string? MoTa { get; set; }

    public decimal? Gia { get; set; }

    public int? TrangThai { get; set; }

    public string? Anh1 { get; set; }

    public string? Anh2 { get; set; }

    public string? Anh3 { get; set; }

    public string? Anh4 { get; set; }

    public string? Anh5 { get; set; }

    public DateTime? NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public bool? An { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<ChiTietKhuyenMai> ChiTietKhuyenMais { get; set; } = new List<ChiTietKhuyenMai>();

    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual LoaiSanPham MaLoaiNavigation { get; set; } = null!;

    public virtual NhaCungCap MaNhaCungCapNavigation { get; set; } = null!;
}
