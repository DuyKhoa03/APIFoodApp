using System;
using System.Collections.Generic;

namespace APIFoodApp.Models;

public partial class DanhGia
{
    public int MaDanhGia { get; set; }

    public int MaNguoiDung { get; set; }

    public int MaSanPham { get; set; }

    public int? SoSao { get; set; }

    public string? NoiDung { get; set; }

    public string? Anh { get; set; }

    public bool? An { get; set; }

    public DateTime? ThoiGianDanhGia { get; set; }

    public DateTime? ThoiGianCapNhat { get; set; }

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
