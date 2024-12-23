using APIFoodApp.Models;

namespace APIFoodApp.Dtos
{
	public class SanPhamDto
	{
		public int MaSanPham { get; set; }

		public int MaLoai { get; set; }

		public int MaNhaCungCap { get; set; }

		public string? TenSanPham { get; set; }

		public string? MoTa { get; set; }

		public decimal? Gia { get; set; }
		public int? SoLuong { get; set; }

		public int? TrangThai { get; set; }

		public string? Anh1 { get; set; }

		public string? Anh2 { get; set; }

		public string? Anh3 { get; set; }

		public string? Anh4 { get; set; }

		public string? Anh5 { get; set; }

		public DateTime? NgayTao { get; set; }

		public DateTime? NgayCapNhat { get; set; }

		public bool? An { get; set; }

		public string? TenLoai { get; set; }

		public string? TenNhaCungCap { get; set; }
		public List<IFormFile>? Images { get; set; } // Danh sách file ảnh tải lên

	}
}
