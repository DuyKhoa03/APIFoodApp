using System;
using System.Collections.Generic;

namespace APIFoodApp.Models;

public partial class ChiTietKhuyenMai
{
    public int MaSanPham { get; set; }

    public int MaKhuyenMai { get; set; }

    public int? SoLuongApDung { get; set; }

    public virtual KhuyenMai MaKhuyenMaiNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
