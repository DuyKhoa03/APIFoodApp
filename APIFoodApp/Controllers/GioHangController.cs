using APIFoodApp.Dtos;
using APIFoodApp.Models;
using Microsoft.AspNetCore.Authorization;
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
		/// Lấy giỏ hàng theo mã người dùng.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Authorize(Roles = "Admin, User")]
		[HttpGet("{id}")]
		public async Task<ActionResult<GioHangDto>> GetByUserId(int id)
		{
			var gioHang = await _context.GioHangs
										.Include(gh => gh.MaNguoiDungNavigation)
										.Include(gh => gh.MaSanPhamNavigation)
										.Where(gh => gh.MaNguoiDung == id)
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
		[Authorize(Roles = "Admin, User")]
		[HttpPost]
		public async Task<ActionResult<GioHang>> CreateGioHang(GioHangDto newGioHangDto)
		{
			if (newGioHangDto == null)
			{
				return BadRequest("GioHang data is null.");
			}

			// Kiểm tra xem sản phẩm có tồn tại không
			var sanPham = await _context.SanPhams.FindAsync(newGioHangDto.MaSanPham);
			if (sanPham == null)
			{
				return NotFound("SanPham không tồn tại.");
			}

			// Kiểm tra xem người dùng có tồn tại không
			var nguoiDung = await _context.NguoiDungs.FindAsync(newGioHangDto.MaNguoiDung);
			if (nguoiDung == null)
			{
				return NotFound("NguoiDung không tồn tại.");
			}

			// Kiểm tra nếu sản phẩm đã tồn tại trong giỏ hàng của người dùng
			var existingGioHang = await _context.GioHangs
				.FirstOrDefaultAsync(gh => gh.MaSanPham == newGioHangDto.MaSanPham
										&& gh.MaNguoiDung == newGioHangDto.MaNguoiDung);

			if (existingGioHang != null)
			{
				// Nếu sản phẩm đã tồn tại, cộng thêm số lượng
				existingGioHang.SoLuong += newGioHangDto.SoLuong ?? 1;

				try
				{
					_context.GioHangs.Update(existingGioHang);
					await _context.SaveChangesAsync();
				}
				catch (Exception ex)
				{
					return StatusCode(500, $"Internal server error: {ex.Message}");
				}

				// Trả về giỏ hàng đã cập nhật
				return Ok(existingGioHang);
			}
			else
			{
				// Nếu sản phẩm chưa tồn tại, thêm sản phẩm mới vào giỏ hàng
				var newGioHang = new GioHang
				{
					MaSanPham = newGioHangDto.MaSanPham,
					MaNguoiDung = newGioHangDto.MaNguoiDung,
					SoLuong = newGioHangDto.SoLuong ?? 1 // Mặc định số lượng là 1 nếu không chỉ định
				};

				try
				{
					_context.GioHangs.Add(newGioHang);
					await _context.SaveChangesAsync();
				}
				catch (Exception ex)
				{
					return StatusCode(500, $"Internal server error: {ex.Message}");
				}

				// Trả về giỏ hàng mới tạo
				return CreatedAtAction(nameof(GetById), new { id = newGioHang.MaGioHang }, newGioHang);
			}
		}


		/// <summary>
		/// Cập nhật thông tin sản phẩm trong giỏ hàng.
		/// </summary>
		/// <param name="id">ID của giỏ hàng cần cập nhật.</param>
		/// <param name="updatedGioHangDto">Thông tin sản phẩm trong giỏ hàng cần cập nhật.</param>
		/// <returns>Không trả về nội dung nếu cập nhật thành công.</returns>
		[Authorize(Roles = "Admin, User")]
		[HttpPut]
		public async Task<IActionResult> UpdateGioHang(GioHangDto updatedGioHangDto)
		{
			if (updatedGioHangDto == null)
			{
				return BadRequest("GioHang data is null.");
			}

			// Kiểm tra nếu sản phẩm và người dùng được cung cấp
			if (updatedGioHangDto.MaSanPham == null || updatedGioHangDto.MaNguoiDung == null)
			{
				return BadRequest("MaSanPham and MaNguoiDung are required.");
			}

			// Tìm giỏ hàng cần cập nhật dựa vào Mã Sản Phẩm và Mã Người Dùng
			var existingGioHang = await _context.GioHangs
				.FirstOrDefaultAsync(gh => gh.MaSanPham == updatedGioHangDto.MaSanPham
										&& gh.MaNguoiDung == updatedGioHangDto.MaNguoiDung);

			if (existingGioHang == null)
			{
				return NotFound("GioHang not found.");
			}

			// Kiểm tra sản phẩm có tồn tại không
			var sanPham = await _context.SanPhams.FindAsync(updatedGioHangDto.MaSanPham);
			if (sanPham == null)
			{
				return NotFound("SanPham không tồn tại.");
			}

			// Kiểm tra người dùng có tồn tại không
			var nguoiDung = await _context.NguoiDungs.FindAsync(updatedGioHangDto.MaNguoiDung);
			if (nguoiDung == null)
			{
				return NotFound("NguoiDung không tồn tại.");
			}

			// Cập nhật thông tin giỏ hàng
			existingGioHang.SoLuong = updatedGioHangDto.SoLuong ?? existingGioHang.SoLuong;

			try
			{
				_context.GioHangs.Update(existingGioHang);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException ex)
			{
				return Conflict($"Concurrency conflict: {ex.Message}");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}

			// Trả về thông tin giỏ hàng đã cập nhật
			return Ok(existingGioHang);
		}

		/// <summary>
		/// Xóa một sản phẩm khỏi giỏ hàng.
		/// </summary>
		/// <param name="id">ID của sản phẩm trong giỏ hàng cần xóa.</param>
		/// <returns>Không trả về nội dung nếu xóa thành công.</returns>
		[Authorize(Roles = "Admin, User")]
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
		[Authorize(Roles = "Admin, User")]
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
