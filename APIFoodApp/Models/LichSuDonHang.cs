using System;
using System.Collections.Generic;

namespace APIFoodApp.Models;

public partial class LichSuDonHang
{
    public int MaLichSu { get; set; }

    public int MaDonHang { get; set; }

    public DateTime? NgayTao { get; set; }

    public int? TrangThai { get; set; }

    public string? GhiChu { get; set; }

    public bool? An { get; set; }

    public virtual DonHang MaDonHangNavigation { get; set; } = null!;
}
