using System;
using System.Collections.Generic;

namespace APIFoodApp.Models;

public partial class NguoiDung
{
    public int MaNguoiDung { get; set; }

    public string? TenNguoiDung { get; set; }

    public string? Email { get; set; }

    public string SoDienThoai { get; set; } = null!;

    public string? Anh { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public int? Quyen { get; set; }

    public bool? An { get; set; }

    public DateTime? NgayDangKy { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    public virtual ICollection<DiaChi> DiaChis { get; set; } = new List<DiaChi>();

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();
}
