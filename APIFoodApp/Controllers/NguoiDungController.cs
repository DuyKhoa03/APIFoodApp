﻿using APIFoodApp.Dtos;
using APIFoodApp.Models;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;

namespace APIFoodApp.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class NguoiDungController : ControllerBase
	{
		private readonly ILogger<NguoiDungController> _logger;
		private readonly FoodAppContext _context;
		private readonly Cloudinary _cloudinary;
		public NguoiDungController(ILogger<NguoiDungController> logger, FoodAppContext context, Cloudinary cloudinary)
		{
			_logger = logger;
			_context = context;
			_cloudinary = cloudinary;
		}

		/// <summary>
		/// Lấy danh sách tất cả người dùng.
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<NguoiDungDto>>> Get()
		{
			var nguoiDungs = await _context.NguoiDungs
											.Where(nd => nd.An == false)
										   .Select(nd => new NguoiDungDto
										   {
											   MaNguoiDung = nd.MaNguoiDung,
											   TenNguoiDung = nd.TenNguoiDung,
											   Email = nd.Email,
											   SoDienThoai = nd.SoDienThoai,
											   Anh = nd.Anh,
											   TenDangNhap = nd.TenDangNhap,
											   MatKhau = null, // Không trả về mật khẩu
											   Quyen = nd.Quyen,
											   An = nd.An
										   })
										   .ToListAsync();

			return Ok(nguoiDungs);
		}

		/// <summary>
		/// Lấy thông tin chi tiết của người dùng theo ID.
		/// </summary>
		[HttpGet("{id}")]
		public async Task<ActionResult<NguoiDungDto>> GetById(int id)
		{
			var nguoiDung = await _context.NguoiDungs
										  .Where(nd => nd.MaNguoiDung == id && nd.An == false)
										  .Select(nd => new NguoiDungDto
										  {
											  MaNguoiDung = nd.MaNguoiDung,
											  TenNguoiDung = nd.TenNguoiDung,
											  Email = nd.Email,
											  SoDienThoai = nd.SoDienThoai,
											  Anh = nd.Anh,
											  TenDangNhap = nd.TenDangNhap,
											  MatKhau = null, // Không trả về mật khẩu
											  Quyen = nd.Quyen,
											  An = nd.An
										  })
										  .FirstOrDefaultAsync();

			if (nguoiDung == null)
			{
				return NotFound("User not found.");
			}

			return Ok(nguoiDung);
		}

		///// <summary>
		///// Tạo mới một người dùng.
		///// </summary>
		[HttpPost]
		public async Task<ActionResult> CreateNguoiDung([FromForm] NguoiDungDto newNguoiDungDto)
		{
			if (newNguoiDungDto == null || string.IsNullOrWhiteSpace(newNguoiDungDto.MatKhau))
			{
				return BadRequest("Invalid user data or password is missing.");
			}

			// Upload ảnh lên Cloudinary
			if (newNguoiDungDto.Img != null && newNguoiDungDto.Img.Length > 0)
			{
				var uploadParams = new ImageUploadParams
				{
					File = new FileDescription(newNguoiDungDto.Img.FileName, newNguoiDungDto.Img.OpenReadStream()),
					Folder = "user-images", // Tên thư mục Cloudinary
					Transformation = new Transformation().Crop("limit").Width(300).Height(300)
				};

				var uploadResult = await _cloudinary.UploadAsync(uploadParams);
				if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
				{
					return BadRequest("Failed to upload image to Cloudinary.");
				}

				// Gán URL ảnh từ Cloudinary
				newNguoiDungDto.Anh = uploadResult.SecureUrl.ToString();
			}

			// Mã hóa mật khẩu
			var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newNguoiDungDto.MatKhau);

			var newNguoiDung = new NguoiDung
			{
				TenNguoiDung = newNguoiDungDto.TenNguoiDung,
				Email = newNguoiDungDto.Email,
				SoDienThoai = newNguoiDungDto.SoDienThoai,
				Anh = newNguoiDungDto.Anh,
				TenDangNhap = newNguoiDungDto.TenDangNhap,
				MatKhau = hashedPassword,
				Quyen = newNguoiDungDto.Quyen,
				An = false
			};

			_context.NguoiDungs.Add(newNguoiDung);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById), new { id = newNguoiDung.MaNguoiDung }, newNguoiDung);
		}
		///// <summary>
		///// Cập nhật thông tin người dùng.
		///// </summary>
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateNguoiDung(int id, [FromForm] NguoiDungDto updatedNguoiDungDto)
		{
			var existingNguoiDung = await _context.NguoiDungs.FindAsync(id);

			if (existingNguoiDung == null)
			{
				return NotFound("User not found.");
			}

			// Cập nhật thông tin khác
			existingNguoiDung.TenNguoiDung = updatedNguoiDungDto.TenNguoiDung;
			existingNguoiDung.Email = updatedNguoiDungDto.Email;
			existingNguoiDung.SoDienThoai = updatedNguoiDungDto.SoDienThoai;
			existingNguoiDung.TenDangNhap = updatedNguoiDungDto.TenDangNhap;
			// Upload ảnh mới lên Cloudinary
			if (updatedNguoiDungDto.Img != null && updatedNguoiDungDto.Img.Length > 0)
			{
				// Xóa ảnh cũ trên Cloudinary nếu có
				if (!string.IsNullOrEmpty(existingNguoiDung.Anh))
				{
					var publicId = new Uri(existingNguoiDung.Anh).Segments.Last().Split('.')[0]; // Trích xuất Public ID từ URL
					var deletionParams = new DeletionParams(publicId);
					await _cloudinary.DestroyAsync(deletionParams);
				}

				var uploadParams = new ImageUploadParams
				{
					File = new FileDescription(updatedNguoiDungDto.Img.FileName, updatedNguoiDungDto.Img.OpenReadStream()),
					Folder = "user-images",
					Transformation = new Transformation().Crop("limit").Width(300).Height(300)
				};

				var uploadResult = await _cloudinary.UploadAsync(uploadParams);
				if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
				{
					return BadRequest("Failed to upload image to Cloudinary.");
				}

				existingNguoiDung.Anh = uploadResult.SecureUrl.ToString();
			}
			// Mã hóa lại mật khẩu nếu có thay đổi
			if (!string.IsNullOrWhiteSpace(updatedNguoiDungDto.MatKhau))
			{
				existingNguoiDung.MatKhau = BCrypt.Net.BCrypt.HashPassword(updatedNguoiDungDto.MatKhau);
			}

			await _context.SaveChangesAsync();

			return NoContent();
		}

		/// <summary>
		/// Xóa một người dùng dựa vào ID.
		/// </summary>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteNguoiDung(int id)
		{
			var nguoiDung = await _context.NguoiDungs.FindAsync(id);
			if (nguoiDung == null)
			{
				return NotFound("User not found.");
			}

			nguoiDung.An = true;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		/// <summary>
		/// Đăng nhập với tên đăng nhập và mật khẩu.
		/// </summary>
		[AllowAnonymous] // Cho phép truy cập không cần đăng nhập
		[HttpPost]
		public async Task<IActionResult> Login(string username, string password)
		{
			var nd = await _context.NguoiDungs.SingleOrDefaultAsync(u => u.TenDangNhap == username);

			if (nd == null)
			{
				return NotFound("Username không tồn tại.");
			}

			// Kiểm tra mật khẩu
			if (!BCrypt.Net.BCrypt.Verify(password, nd.MatKhau))
			{
				return Unauthorized("Mật khẩu không đúng.");
			}

			// Cập nhật thời gian đăng nhập cuối cùng

			// Trả về thông tin cơ bản của người dùng
			return Ok(new
			{
				MaNguoiDung = nd.MaNguoiDung,
				TenNguoiDung = nd.TenNguoiDung,
				Email = nd.Email,
				SoDienThoai = nd.SoDienThoai,
				Anh = nd.Anh,
				TenDangNhap = nd.TenDangNhap,
				Quyen = nd.Quyen,
				An = nd.An
			});
		}
		/// <summary>
		/// Tìm kiếm người dùng theo từ khóa.
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (tên, email hoặc số điện thoại).</param>
		[HttpGet("{keyword}")]
		public async Task<ActionResult<IEnumerable<NguoiDungDto>>> Search(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return BadRequest("Keyword cannot be empty.");
			}

			var searchResults = await _context.NguoiDungs
											  .Where(nd =>
												  nd.TenNguoiDung.Contains(keyword) ||
												  nd.Email.Contains(keyword) ||
												  nd.SoDienThoai.Contains(keyword) || nd.MaNguoiDung.ToString() == keyword.Trim()
												  && nd.An == false)
											  .Select(nd => new NguoiDungDto
											  {
												  MaNguoiDung = nd.MaNguoiDung,
												  TenNguoiDung = nd.TenNguoiDung,
												  Email = nd.Email,
												  SoDienThoai = nd.SoDienThoai,
												  Anh = nd.Anh,
												  TenDangNhap = nd.TenDangNhap,
												  Quyen = nd.Quyen,
												  An = nd.An
											  })
											  .ToListAsync();

			return Ok(searchResults);
		}

	}
}
