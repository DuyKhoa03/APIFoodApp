using APIFoodApp.Models;

namespace APIFoodApp.Dtos
{
	public class ChiTietDonHangDto
	{
		public int MaSanPham { get; set; }

		public int MaDonHang { get; set; }

		public int? SoLuong { get; set; }

		public string? GhiChu { get; set; }

		public int? MaKhuyenMaiApDung { get; set; }

		public string? TenSanPham { get; set; }
	}
}
