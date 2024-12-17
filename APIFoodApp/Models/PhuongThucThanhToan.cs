using System;
using System.Collections.Generic;

namespace APIFoodApp.Models;

public partial class PhuongThucThanhToan
{
    public int MaPhuongThuc { get; set; }

    public string? Ten { get; set; }

    public string? MoTa { get; set; }

    public bool? An { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
