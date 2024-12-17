﻿using APIFoodApp.Models;

namespace APIFoodApp.Dtos
{
	public class ThongBaoDto
	{
		public int MaThongBao { get; set; }

		public int MaNguoiDung { get; set; }

		public string? Ten { get; set; }

		public string? MoTa { get; set; }

		public string? NoiDung { get; set; }

		public string? TheLoai { get; set; }

		public string? DieuKienKichHoat { get; set; }

		public bool? An { get; set; }

		public DateTime? NgayTao { get; set; }

		public DateTime? NgayCapNhat { get; set; }

		public string? TenNguoiDung { get; set; }
	}
}
