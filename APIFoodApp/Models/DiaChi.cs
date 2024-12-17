using System;
using System.Collections.Generic;

namespace APIFoodApp.Models;

public partial class DiaChi
{
    public int MaDiaChi { get; set; }

    public int MaNguoiDung { get; set; }

    public string? Ten { get; set; }

    public string? DiaChi1 { get; set; }

    public bool? An { get; set; }

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
