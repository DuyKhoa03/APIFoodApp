using APIFoodApp.Dtos;
using APIFoodApp.Models;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace APIFoodApp.Controllers
{
	[Authorize(Roles = "Admin, User")]
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class DanhGiaController : ControllerBase
	{
		private readonly ILogger<DanhGiaController> _logger;
		private readonly FoodAppContext _context;
		private readonly Cloudinary _cloudinary;
		public DanhGiaController(ILogger<DanhGiaController> logger, FoodAppContext context, Cloudinary cloudinary)
		{
			_logger = logger;
			_context = context;
			_cloudinary = cloudinary;
		}

		/// <summary>
		/// Lấy danh sách tất cả các đánh giá, chỉ bao gồm những đánh giá không bị ẩn.
		/// </summary>
		/// <returns>Danh sách các đánh giá.</returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<DanhGiaDto>>> Get()
		{
			var danhGias = await _context.DanhGia
										 .Include(dg => dg.MaNguoiDungNavigation)
										 .Include(dg => dg.MaSanPhamNavigation)
										 .Where(dg => dg.An == false)
										 .Select(dg => new DanhGiaDto
										 {
											 MaDanhGia = dg.MaDanhGia,
											 MaNguoiDung = dg.MaNguoiDung,
											 MaSanPham = dg.MaSanPham,
											 SoSao = dg.SoSao,
											 NoiDung = dg.NoiDung,
											 Anh = dg.Anh,
											 An = dg.An,
											 ThoiGianDanhGia = dg.ThoiGianDanhGia,
											 ThoiGianCapNhat = dg.ThoiGianCapNhat,
											 TenNguoiDung = dg.MaNguoiDungNavigation.TenNguoiDung,
											 TenSanPham = dg.MaSanPhamNavigation.TenSanPham
										 })
										 .ToListAsync();

			return Ok(danhGias);
		}

		/// <summary>
		/// Lấy thông tin chi tiết của một đánh giá theo ID.
		/// </summary>
		/// <param name="id">ID của đánh giá cần lấy.</param>
		/// <returns>Thông tin của đánh giá nếu tìm thấy.</returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<DanhGiaDto>> GetById(int id)
		{
			var danhGia = await _context.DanhGia
										.Include(dg => dg.MaNguoiDungNavigation)
										.Include(dg => dg.MaSanPhamNavigation)
										.Where(dg => dg.MaDanhGia == id && dg.An == false)
										.Select(dg => new DanhGiaDto
										{
											MaDanhGia = dg.MaDanhGia,
											MaNguoiDung = dg.MaNguoiDung,
											MaSanPham = dg.MaSanPham,
											SoSao = dg.SoSao,
											NoiDung = dg.NoiDung,
											Anh = dg.Anh,
											An = dg.An,
											ThoiGianDanhGia = dg.ThoiGianDanhGia,
											ThoiGianCapNhat = dg.ThoiGianCapNhat,
											TenNguoiDung = dg.MaNguoiDungNavigation.TenNguoiDung,
											TenSanPham = dg.MaSanPhamNavigation.TenSanPham
										})
										.FirstOrDefaultAsync();

			if (danhGia == null)
			{
				return NotFound("DanhGia not found.");
			}

			return Ok(danhGia);
		}

		/// <summary>
		/// Tạo mới một đánh giá.
		/// </summary>
		/// <param name="newDanhGiaDto">Thông tin đánh giá mới cần tạo.</param>
		/// <returns>Đánh giá vừa được tạo nếu thành công.</returns>
		[HttpPost]
		public async Task<ActionResult<DanhGia>> CreateDanhGia([FromForm] DanhGiaDto newDanhGiaDto)
		{
			if (newDanhGiaDto == null)
			{
				return BadRequest("DanhGia data is null.");
			}
			// Upload ảnh lên Cloudinary
			if (newDanhGiaDto.Img != null && newDanhGiaDto.Img.Length > 0)
			{
				var uploadParams = new ImageUploadParams
				{
					File = new FileDescription(newDanhGiaDto.Img.FileName, newDanhGiaDto.Img.OpenReadStream()),
					Folder = "vote-images", // Tên thư mục Cloudinary
					Transformation = new Transformation().Crop("limit").Width(300).Height(300)
				};

				var uploadResult = await _cloudinary.UploadAsync(uploadParams);
				if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
				{
					return BadRequest("Failed to upload image to Cloudinary.");
				}

				// Gán URL ảnh từ Cloudinary
				newDanhGiaDto.Anh = uploadResult.SecureUrl.ToString();
			}
			var newDanhGia = new DanhGium
			{
				MaNguoiDung = newDanhGiaDto.MaNguoiDung,
				MaSanPham = newDanhGiaDto.MaSanPham,
				SoSao = newDanhGiaDto.SoSao,
				NoiDung = newDanhGiaDto.NoiDung,
				Anh = newDanhGiaDto.Anh,
				An = false,
				ThoiGianDanhGia = DateTime.Now,
				ThoiGianCapNhat = DateTime.Now
			};

			_context.DanhGia.Add(newDanhGia);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById), new { id = newDanhGia.MaDanhGia }, newDanhGia);
		}

		/// <summary>
		/// Cập nhật thông tin của một đánh giá dựa vào ID.
		/// </summary>
		/// <param name="id">ID của đánh giá cần cập nhật.</param>
		/// <param name="updatedDanhGiaDto">Thông tin đánh giá cần cập nhật.</param>
		/// <returns>Không trả về nội dung nếu cập nhật thành công.</returns>
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateDanhGia(int id, DanhGiaDto updatedDanhGiaDto)
		{
			if (updatedDanhGiaDto == null)
			{
				return BadRequest("DanhGia data is null.");
			}

			var existingDanhGia = await _context.DanhGia.FindAsync(id);
			if (existingDanhGia == null)
			{
				return NotFound("DanhGia not found.");
			}

			// Cập nhật các thuộc tính
			existingDanhGia.MaNguoiDung = updatedDanhGiaDto.MaNguoiDung;
			existingDanhGia.MaSanPham = updatedDanhGiaDto.MaSanPham;
			existingDanhGia.SoSao = updatedDanhGiaDto.SoSao;
			existingDanhGia.NoiDung = updatedDanhGiaDto.NoiDung;
			existingDanhGia.Anh = updatedDanhGiaDto.Anh;
			existingDanhGia.An = updatedDanhGiaDto.An ?? false;
			existingDanhGia.ThoiGianDanhGia = updatedDanhGiaDto.ThoiGianDanhGia;
			existingDanhGia.ThoiGianCapNhat = updatedDanhGiaDto.ThoiGianCapNhat;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await _context.DanhGia.AnyAsync(dg => dg.MaDanhGia == id))
				{
					return NotFound("DanhGia not found.");
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		/// <summary>
		/// Xóa (ẩn) một đánh giá dựa vào ID.
		/// </summary>
		/// <param name="id">ID của đánh giá cần xóa.</param>
		/// <returns>Không trả về nội dung nếu xóa thành công.</returns>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteDanhGia(int id)
		{
			var danhGia = await _context.DanhGia.FindAsync(id);
			if (danhGia == null)
			{
				return NotFound("DanhGia not found.");
			}

			// Ẩn đánh giá thay vì xóa vĩnh viễn
			danhGia.An = true;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		/// <summary>
		/// Tìm kiếm đánh giá theo từ khóa.
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (tên người dùng, tên sản phẩm, hoặc nội dung).</param>
		/// <returns>Danh sách các đánh giá phù hợp với từ khóa tìm kiếm.</returns>
		[HttpGet("{keyword}")]
		public async Task<ActionResult<IEnumerable<DanhGiaDto>>> Search(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return BadRequest("Keyword cannot be empty.");
			}

			var searchResults = await _context.DanhGia
											  .Include(dg => dg.MaNguoiDungNavigation)
											  .Include(dg => dg.MaSanPhamNavigation)
											  .Where(dg =>
												  (dg.NoiDung.Contains(keyword) ||
												   dg.MaNguoiDungNavigation.TenNguoiDung.Contains(keyword) ||
												   dg.MaSanPhamNavigation.TenSanPham.Contains(keyword)) &&
												  dg.An == false)
											  .Select(dg => new DanhGiaDto
											  {
												  MaDanhGia = dg.MaDanhGia,
												  MaNguoiDung = dg.MaNguoiDung,
												  MaSanPham = dg.MaSanPham,
												  SoSao = dg.SoSao,
												  NoiDung = dg.NoiDung,
												  Anh = dg.Anh,
												  An = dg.An,
												  ThoiGianDanhGia = dg.ThoiGianDanhGia,
												  ThoiGianCapNhat = dg.ThoiGianCapNhat,
												  TenNguoiDung = dg.MaNguoiDungNavigation.TenNguoiDung,
												  TenSanPham = dg.MaSanPhamNavigation.TenSanPham
											  })
											  .ToListAsync();

			return Ok(searchResults);
		}
	}
}
