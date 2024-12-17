using APIFoodApp.Models;

namespace APIFoodApp.Dtos
{
	public class GioHangDto
	{
		public int MaGioHang { get; set; }

		public int MaSanPham { get; set; }

		public int MaNguoiDung { get; set; }

		public int? SoLuong { get; set; }

		public string? TenNguoiDung { get; set; }

		public string? TenSanPham { get; set; }
	}
}
