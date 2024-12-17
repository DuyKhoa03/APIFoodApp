using System;
using System.Collections.Generic;

namespace APIFoodApp.Models;

public partial class LoaiSanPham
{
    public int MaLoai { get; set; }

    public string? TenLoai { get; set; }

    public string? MoTa { get; set; }

    public bool? An { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
