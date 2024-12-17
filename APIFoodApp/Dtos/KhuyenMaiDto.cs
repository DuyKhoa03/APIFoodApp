using APIFoodApp.Models;

namespace APIFoodApp.Dtos
{
	public class KhuyenMaiDto
	{
		public int MaKhuyenMai { get; set; }

		public int MaLoai { get; set; }

		public string? Ten { get; set; }

		public decimal? GiaTri { get; set; }

		public string? DieuKienApDung { get; set; }

		public DateTime? BatDau { get; set; }

		public DateTime? KetThuc { get; set; }

		public DateTime? NgayTao { get; set; }

		public DateTime? NgayCapNhat { get; set; }

		public bool? An { get; set; }
		// ten loai khuyen mai
		public string? TenLoai { get; set; }

	}
}
