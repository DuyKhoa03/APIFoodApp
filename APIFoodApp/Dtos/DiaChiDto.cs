using APIFoodApp.Models;

namespace APIFoodApp.Dtos
{
	public class DiaChiDto
	{
		public int MaDiaChi { get; set; }

		public int MaNguoiDung { get; set; }

		public string? Ten { get; set; }

		public string? DiaChi { get; set; }

		public string? TenNguoiDung { get; set; }
		public bool? An { get; set; }
	}
}
