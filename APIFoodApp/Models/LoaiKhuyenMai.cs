using System;
using System.Collections.Generic;

namespace APIFoodApp.Models;

public partial class LoaiKhuyenMai
{
    public int MaLoai { get; set; }

    public string? TenLoai { get; set; }

    public string? MoTa { get; set; }

    public bool? An { get; set; }

    public virtual ICollection<KhuyenMai> KhuyenMais { get; set; } = new List<KhuyenMai>();
}
