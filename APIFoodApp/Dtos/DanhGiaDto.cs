using APIFoodApp.Models;

namespace APIFoodApp.Dtos
{
	public class DanhGiaDto
	{
		public int MaDanhGia { get; set; }

		public int MaNguoiDung { get; set; }

		public int MaSanPham { get; set; }

		public int? SoSao { get; set; }

		public string? NoiDung { get; set; }

		public string? Anh { get; set; }

		public bool? An { get; set; }

		public DateTime? ThoiGianDanhGia { get; set; }

		public DateTime? ThoiGianCapNhat { get; set; }

		public string? TenNguoiDung { get; set; }

		public string? TenSanPham { get; set; }
		public IFormFile? Img { get; set; } // Trường này để nhận file ảnh tải lên
	}
}
