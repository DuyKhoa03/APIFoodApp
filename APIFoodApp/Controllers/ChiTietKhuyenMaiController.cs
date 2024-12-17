using APIFoodApp.Dtos;
using APIFoodApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFoodApp.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class ChiTietKhuyenMaiController : ControllerBase
	{
		private readonly ILogger<ChiTietKhuyenMaiController> _logger;
		private readonly FoodAppContext _context;

		public ChiTietKhuyenMaiController(ILogger<ChiTietKhuyenMaiController> logger, FoodAppContext context)
		{
			_logger = logger;
			_context = context;
		}

		/// <summary>
		/// Lấy danh sách tất cả chi tiết khuyến mãi.
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ChiTietKhuyenMaiDto>>> Get()
		{
			var chiTietKhuyenMai = await _context.ChiTietKhuyenMais
												.Include(ct => ct.MaSanPhamNavigation)
												.Include(ct => ct.MaKhuyenMaiNavigation)
												.Select(ct => new ChiTietKhuyenMaiDto
												{
													MaSanPham = ct.MaSanPham,
													MaKhuyenMai = ct.MaKhuyenMai,
													SoLuongApDung = ct.SoLuongApDung,
													TenSanPham = ct.MaSanPhamNavigation.TenSanPham,
													TenKhuyenMai = ct.MaKhuyenMaiNavigation.Ten
												})
												.ToListAsync();

			return Ok(chiTietKhuyenMai);
		}

		/// <summary>
		/// Lấy chi tiết khuyến mãi theo MaSanPham và MaKhuyenMai.
		/// </summary>
		[HttpGet("{maSanPham}/{maKhuyenMai}")]
		public async Task<ActionResult<ChiTietKhuyenMaiDto>> GetById(int maSanPham, int maKhuyenMai)
		{
			var chiTiet = await _context.ChiTietKhuyenMais
										.Include(ct => ct.MaSanPhamNavigation)
										.Include(ct => ct.MaKhuyenMaiNavigation)
										.Where(ct => ct.MaSanPham == maSanPham &&
													 ct.MaKhuyenMai == maKhuyenMai)
										.Select(ct => new ChiTietKhuyenMaiDto
										{
											MaSanPham = ct.MaSanPham,
											MaKhuyenMai = ct.MaKhuyenMai,
											SoLuongApDung = ct.SoLuongApDung,
											TenSanPham = ct.MaSanPhamNavigation.TenSanPham,
											TenKhuyenMai = ct.MaKhuyenMaiNavigation.Ten
										})
										.FirstOrDefaultAsync();

			if (chiTiet == null)
			{
				return NotFound("Chi tiết khuyến mãi không tìm thấy.");
			}

			return Ok(chiTiet);
		}
		/// <summary>
		/// Lấy danh sách chi tiết khuyến mãi theo MaKhuyenMai.
		/// </summary>
		[HttpGet("{maKhuyenMai}")]
		public async Task<ActionResult<ChiTietKhuyenMaiDto>> GetByKhuyenMaiId(int maKhuyenMai)
		{
			var chiTiet = await _context.ChiTietKhuyenMais
										.Include(ct => ct.MaSanPhamNavigation)
										.Include(ct => ct.MaKhuyenMaiNavigation)
										.Where(ct => ct.MaKhuyenMai == maKhuyenMai)
										.Select(ct => new ChiTietKhuyenMaiDto
										{
											MaSanPham = ct.MaSanPham,
											MaKhuyenMai = ct.MaKhuyenMai,
											SoLuongApDung = ct.SoLuongApDung,
											TenSanPham = ct.MaSanPhamNavigation.TenSanPham,
											TenKhuyenMai = ct.MaKhuyenMaiNavigation.Ten
										})
										.ToListAsync();

			if (chiTiet == null)
			{
				return NotFound("Chi tiết khuyến mãi không tìm thấy.");
			}

			return Ok(chiTiet);
		}
		/// <summary>
		/// Lấy danh sách chi tiết khuyến mãi theo MaSanPham .
		/// </summary>
		[HttpGet("{maSanPham}")]
		public async Task<ActionResult<ChiTietKhuyenMaiDto>> GetByProductId(int maSanPham)
		{
			var chiTiet = await _context.ChiTietKhuyenMais
										.Include(ct => ct.MaSanPhamNavigation)
										.Include(ct => ct.MaKhuyenMaiNavigation)
										.Where(ct => ct.MaSanPham == maSanPham)
										.Select(ct => new ChiTietKhuyenMaiDto
										{
											MaSanPham = ct.MaSanPham,
											MaKhuyenMai = ct.MaKhuyenMai,
											SoLuongApDung = ct.SoLuongApDung,
											TenSanPham = ct.MaSanPhamNavigation.TenSanPham,
											TenKhuyenMai = ct.MaKhuyenMaiNavigation.Ten
										})
										.ToListAsync();

			if (chiTiet == null)
			{
				return NotFound("Chi tiết khuyến mãi không tìm thấy.");
			}

			return Ok(chiTiet);
		}
		/// <summary>
		/// Thêm mới chi tiết khuyến mãi.
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> Create(ChiTietKhuyenMaiDto newDto)
		{
			if (newDto == null)
			{
				return BadRequest("Dữ liệu không hợp lệ.");
			}

			var newChiTiet = new ChiTietKhuyenMai
			{
				MaSanPham = newDto.MaSanPham,
				MaKhuyenMai = newDto.MaKhuyenMai,
				SoLuongApDung = newDto.SoLuongApDung
			};

			_context.ChiTietKhuyenMais.Add(newChiTiet);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById),
								   new { maSanPham = newChiTiet.MaSanPham, maKhuyenMai = newChiTiet.MaKhuyenMai },
								   newChiTiet);
		}

		/// <summary>
		/// Cập nhật chi tiết khuyến mãi.
		/// </summary>
		[HttpPut("{maSanPham}/{maKhuyenMai}")]
		public async Task<IActionResult> Update(int maSanPham, int maKhuyenMai, ChiTietKhuyenMaiDto updatedDto)
		{
			if (updatedDto == null)
			{
				return BadRequest("Dữ liệu không hợp lệ.");
			}

			var existing = await _context.ChiTietKhuyenMais
										 .Where(ct => ct.MaSanPham == maSanPham && ct.MaKhuyenMai == maKhuyenMai)
										 .FirstOrDefaultAsync();

			if (existing == null)
			{
				return NotFound("Chi tiết khuyến mãi không tìm thấy.");
			}

			existing.SoLuongApDung = updatedDto.SoLuongApDung;

			await _context.SaveChangesAsync();
			return NoContent();
		}

		/// <summary>
		/// Xóa mềm chi tiết khuyến mãi (An = true).
		/// </summary>
		[HttpDelete("{maSanPham}/{maKhuyenMai}")]
		public async Task<IActionResult> Delete(int maSanPham, int maKhuyenMai)
		{
			var existing = await _context.ChiTietKhuyenMais
										 .Where(ct => ct.MaSanPham == maSanPham && ct.MaKhuyenMai == maKhuyenMai)
										 .FirstOrDefaultAsync();

			if (existing == null)
			{
				return NotFound("Chi tiết khuyến mãi không tìm thấy.");
			}
			_context.Remove(existing);
			await _context.SaveChangesAsync();
			return NoContent();
		}

		/// <summary>
		/// Tìm kiếm chi tiết khuyến mãi theo từ khóa.
		/// </summary>
		[HttpGet("{keyword}")]
		public async Task<ActionResult<IEnumerable<ChiTietKhuyenMaiDto>>> Search(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return BadRequest("Từ khóa tìm kiếm không hợp lệ.");
			}

			var results = await _context.ChiTietKhuyenMais
										.Include(ct => ct.MaSanPhamNavigation)
										.Include(ct => ct.MaKhuyenMaiNavigation)
										.Where(ct => (ct.MaSanPhamNavigation.TenSanPham.Contains(keyword) ||
													 ct.MaKhuyenMaiNavigation.Ten.Contains(keyword)))
										.Select(ct => new ChiTietKhuyenMaiDto
										{
											MaSanPham = ct.MaSanPham,
											MaKhuyenMai = ct.MaKhuyenMai,
											SoLuongApDung = ct.SoLuongApDung,
											TenSanPham = ct.MaSanPhamNavigation.TenSanPham,
											TenKhuyenMai = ct.MaKhuyenMaiNavigation.Ten
										})
										.ToListAsync();

			return Ok(results);
		}
	}
}
