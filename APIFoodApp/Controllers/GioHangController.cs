using APIFoodApp.Dtos;
using APIFoodApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFoodApp.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class GioHangController : ControllerBase
	{
		private readonly ILogger<GioHangController> _logger;
		private readonly FoodAppContext _context;

		public GioHangController(ILogger<GioHangController> logger, FoodAppContext context)
		{
			_logger = logger;
			_context = context;
		}

		/// <summary>
		/// Lấy danh sách tất cả các sản phẩm trong giỏ hàng.
		/// </summary>
		/// <returns>Danh sách các sản phẩm trong giỏ hàng.</returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<GioHangDto>>> Get()
		{
			var gioHangs = await _context.GioHangs
										 .Include(gh => gh.MaNguoiDungNavigation)
										 .Include(gh => gh.MaSanPhamNavigation)
										 .Select(gh => new GioHangDto
										 {
											 MaGioHang = gh.MaGioHang,
											 MaSanPham = gh.MaSanPham,
											 MaNguoiDung = gh.MaNguoiDung,
											 SoLuong = gh.SoLuong,
											 TenNguoiDung = gh.MaNguoiDungNavigation.TenNguoiDung,
											 TenSanPham = gh.MaSanPhamNavigation.TenSanPham
										 })
										 .ToListAsync();

			return Ok(gioHangs);
		}

		/// <summary>
		/// Lấy thông tin chi tiết của một sản phẩm trong giỏ hàng theo ID.
		/// </summary>
		/// <param name="id">ID của giỏ hàng cần lấy.</param>
		/// <returns>Thông tin chi tiết của giỏ hàng.</returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<GioHangDto>> GetById(int id)
		{
			var gioHang = await _context.GioHangs
										.Include(gh => gh.MaNguoiDungNavigation)
										.Include(gh => gh.MaSanPhamNavigation)
										.Where(gh => gh.MaGioHang == id)
										.Select(gh => new GioHangDto
										{
											MaGioHang = gh.MaGioHang,
											MaSanPham = gh.MaSanPham,
											MaNguoiDung = gh.MaNguoiDung,
											SoLuong = gh.SoLuong,
											TenNguoiDung = gh.MaNguoiDungNavigation.TenNguoiDung,
											TenSanPham = gh.MaSanPhamNavigation.TenSanPham
										})
										.FirstOrDefaultAsync();

			if (gioHang == null)
			{
				return NotFound("GioHang not found.");
			}

			return Ok(gioHang);
		}

		/// <summary>
		/// Tạo mới một mục giỏ hàng.
		/// </summary>
		/// <param name="newGioHangDto">Thông tin sản phẩm trong giỏ hàng cần thêm.</param>
		/// <returns>Sản phẩm vừa được thêm vào giỏ hàng.</returns>
		[HttpPost]
		public async Task<ActionResult<GioHang>> CreateGioHang(GioHangDto newGioHangDto)
		{
			if (newGioHangDto == null)
			{
				return BadRequest("GioHang data is null.");
			}

			var newGioHang = new GioHang
			{
				MaSanPham = newGioHangDto.MaSanPham,
				MaNguoiDung = newGioHangDto.MaNguoiDung,
				SoLuong = newGioHangDto.SoLuong ?? 1 // Mặc định số lượng là 1 nếu không chỉ định
			};

			_context.GioHangs.Add(newGioHang);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById), new { id = newGioHang.MaGioHang }, newGioHang);
		}

		/// <summary>
		/// Cập nhật thông tin sản phẩm trong giỏ hàng.
		/// </summary>
		/// <param name="id">ID của giỏ hàng cần cập nhật.</param>
		/// <param name="updatedGioHangDto">Thông tin sản phẩm trong giỏ hàng cần cập nhật.</param>
		/// <returns>Không trả về nội dung nếu cập nhật thành công.</returns>
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateGioHang(int id, GioHangDto updatedGioHangDto)
		{
			if (updatedGioHangDto == null)
			{
				return BadRequest("GioHang data is null.");
			}

			var existingGioHang = await _context.GioHangs.FindAsync(id);
			if (existingGioHang == null)
			{
				return NotFound("GioHang not found.");
			}

			// Cập nhật các thuộc tính
			existingGioHang.MaSanPham = updatedGioHangDto.MaSanPham;
			existingGioHang.MaNguoiDung = updatedGioHangDto.MaNguoiDung;
			existingGioHang.SoLuong = updatedGioHangDto.SoLuong ?? existingGioHang.SoLuong;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await _context.GioHangs.AnyAsync(gh => gh.MaGioHang == id))
				{
					return NotFound("GioHang not found.");
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		/// <summary>
		/// Xóa một sản phẩm khỏi giỏ hàng.
		/// </summary>
		/// <param name="id">ID của sản phẩm trong giỏ hàng cần xóa.</param>
		/// <returns>Không trả về nội dung nếu xóa thành công.</returns>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteGioHang(int id)
		{
			var gioHang = await _context.GioHangs.FindAsync(id);
			if (gioHang == null)
			{
				return NotFound("GioHang not found.");
			}

			_context.GioHangs.Remove(gioHang);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		/// <summary>
		/// Tìm kiếm sản phẩm trong giỏ hàng theo từ khóa.
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (tên người dùng hoặc tên sản phẩm).</param>
		/// <returns>Danh sách các sản phẩm trong giỏ hàng phù hợp với từ khóa tìm kiếm.</returns>
		[HttpGet("{keyword}")]
		public async Task<ActionResult<IEnumerable<GioHangDto>>> Search(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return BadRequest("Keyword cannot be empty.");
			}

			var searchResults = await _context.GioHangs
											  .Where(gh =>
												  gh.MaGioHang.ToString().Contains(keyword) ||
												  gh.MaSanPhamNavigation.TenSanPham.Contains(keyword))
											  .Select(gh => new GioHangDto
											  {
												  MaGioHang = gh.MaGioHang,
												  MaSanPham = gh.MaSanPham,
												  MaNguoiDung = gh.MaNguoiDung,
												  SoLuong = gh.SoLuong,
												  TenNguoiDung = gh.MaNguoiDungNavigation.TenNguoiDung,
												  TenSanPham = gh.MaSanPhamNavigation.TenSanPham
											  })
											  .ToListAsync();

			return Ok(searchResults);
		}
	}
}
