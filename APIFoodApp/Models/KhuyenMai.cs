using System;
using System.Collections.Generic;

namespace APIFoodApp.Models;

public partial class KhuyenMai
{
    public int MaKhuyenMai { get; set; }

    public int MaLoai { get; set; }

    public string? Ten { get; set; }

    public decimal? GiaTri { get; set; }

    public string? DieuKienApDung { get; set; }

    public DateTime? BatDau { get; set; }

    public DateTime? KetThuc { get; set; }

    public DateTime? NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public bool? An { get; set; }

    public virtual ICollection<ChiTietKhuyenMai> ChiTietKhuyenMais { get; set; } = new List<ChiTietKhuyenMai>();

    public virtual LoaiKhuyenMai MaLoaiNavigation { get; set; } = null!;
}
