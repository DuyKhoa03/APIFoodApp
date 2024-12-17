using System;
using System.Collections.Generic;

namespace APIFoodApp.Models;

public partial class ChiTietDonHang
{
    public int MaSanPham { get; set; }

    public int MaDonHang { get; set; }

    public int? SoLuong { get; set; }

    public string? GhiChu { get; set; }

    public int? MaKhuyenMaiApDung { get; set; }

    public virtual DonHang MaDonHangNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
