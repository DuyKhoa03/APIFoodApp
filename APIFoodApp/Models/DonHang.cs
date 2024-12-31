using System;
using System.Collections.Generic;

namespace APIFoodApp.Models;

public partial class DonHang
{
    public int MaDonHang { get; set; }

    public int MaNguoiDung { get; set; }

    public int MaPhuongThuc { get; set; }

    public decimal? TongTien { get; set; }

    public int? TrangThai { get; set; }

    public DateTime? NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public bool? An { get; set; }

    public int MaDiaChi { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<LichSuDonHang> LichSuDonHangs { get; set; } = new List<LichSuDonHang>();

    public virtual DiaChi MaDiaChiNavigation { get; set; } = null!;

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;

    public virtual PhuongThucThanhToan MaPhuongThucNavigation { get; set; } = null!;
}
