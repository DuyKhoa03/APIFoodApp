using APIFoodApp.Models;

namespace APIFoodApp.Dtos
{
	public class DonHangDto
	{
		public int MaDonHang { get; set; }

		public int MaNguoiDung { get; set; }

		public int MaPhuongThuc { get; set; }
		public int MaDiaChi { get; set; }

		public decimal? TongTien { get; set; }

		public int? TrangThai { get; set; }

		public DateTime? NgayTao { get; set; }

		public DateTime? NgayCapNhat { get; set; }

		public bool? An { get; set; }

		public string? TenNguoiDung { get; set; }

		public string? TenPhuongThuc { get; set; }
		public string? TenDiaChi { get; set; }
	}
}
