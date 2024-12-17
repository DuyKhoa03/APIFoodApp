namespace APIFoodApp.Dtos
{
	public class NguoiDungDto
	{
		public int MaNguoiDung { get; set; }

		public string? TenNguoiDung { get; set; }

		public string? Email { get; set; }

		public string SoDienThoai { get; set; } = null!;

		public string? Anh { get; set; }

		public string TenDangNhap { get; set; } = null!;

		public string MatKhau { get; set; } = null!;

		public int? Quyen { get; set; }

		public bool? An { get; set; }
		public IFormFile? Img { get; set; } // Trường này để nhận file ảnh tải lên
	}
}
