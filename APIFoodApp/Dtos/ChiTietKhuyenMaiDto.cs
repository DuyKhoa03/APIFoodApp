using APIFoodApp.Models;

namespace APIFoodApp.Dtos
{
	public class ChiTietKhuyenMaiDto
	{
		public int MaSanPham { get; set; }

		public int MaKhuyenMai { get; set; }

		public int? SoLuongApDung { get; set; }

		public string? TenKhuyenMai { get; set; }

		public string? TenSanPham { get; set; }
	}
}
