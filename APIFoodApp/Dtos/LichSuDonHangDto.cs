using APIFoodApp.Models;

namespace APIFoodApp.Dtos
{
	public class LichSuDonHangDto
	{
		public int MaLichSu { get; set; }

		public int MaDonHang { get; set; }

		public DateTime? NgayTao { get; set; }

		public int? TrangThai { get; set; }

		public string? GhiChu { get; set; }
		public bool? An { get; set; }

	}
}
