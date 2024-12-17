using APIFoodApp.Dtos;
using APIFoodApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFoodApp.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class ChiTietDonHangController : ControllerBase
	{
		private readonly ILogger<ChiTietDonHangController> _logger;
		private readonly FoodAppContext _context;

		public ChiTietDonHangController(ILogger<ChiTietDonHangController> logger, FoodAppContext context)
		{
			_logger = logger;
			_context = context;
		}

		/// <summary>
		/// Lấy danh sách tất cả các chi tiết đơn hàng.
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ChiTietDonHangDto>>> Get()
		{
			var chiTietDonHangs = await _context.ChiTietDonHangs
												.Include(ct => ct.MaSanPhamNavigation)
												.Select(ct => new ChiTietDonHangDto
												{
													MaSanPham = ct.MaSanPham,
													MaDonHang = ct.MaDonHang,
													SoLuong = ct.SoLuong,
													GhiChu = ct.GhiChu,
													MaKhuyenMaiApDung = ct.MaKhuyenMaiApDung,
													TenSanPham = ct.MaSanPhamNavigation.TenSanPham
												})
												.ToListAsync();

			return Ok(chiTietDonHangs);
		}

		/// <summary>
		/// Lấy thông tin chi tiết của một chi tiết đơn hàng theo ID.
		/// </summary>
		[HttpGet("{maDonHang}/{maSanPham}")]
		public async Task<ActionResult<ChiTietDonHangDto>> GetById(int maDonHang, int maSanPham)
		{
			var chiTietDonHang = await _context.ChiTietDonHangs
											   .Include(ct => ct.MaSanPhamNavigation)
											   .Where(ct => ct.MaDonHang == maDonHang &&
															ct.MaSanPham == maSanPham)
											   .Select(ct => new ChiTietDonHangDto
											   {
												   MaSanPham = ct.MaSanPham,
												   MaDonHang = ct.MaDonHang,
												   SoLuong = ct.SoLuong,
												   GhiChu = ct.GhiChu,
												   MaKhuyenMaiApDung = ct.MaKhuyenMaiApDung,
												   TenSanPham = ct.MaSanPhamNavigation.TenSanPham
											   })
											   .FirstOrDefaultAsync();

			if (chiTietDonHang == null)
			{
				return NotFound("Chi tiết đơn hàng không tìm thấy.");
			}

			return Ok(chiTietDonHang);
		}
		/// <summary>
		/// Lấy thông tin danh sách chi tiết của đơn hàng theo ID đơn hàng.
		/// </summary>
		[HttpGet("{maDonHang}")]
		public async Task<ActionResult<ChiTietDonHangDto>> GetByOrderId(int maDonHang)
		{
			var chiTietDonHang = await _context.ChiTietDonHangs
											   .Include(ct => ct.MaSanPhamNavigation)
											   .Where(ct => ct.MaDonHang == maDonHang)
											   .Select(ct => new ChiTietDonHangDto
											   {
												   MaSanPham = ct.MaSanPham,
												   MaDonHang = ct.MaDonHang,
												   SoLuong = ct.SoLuong,
												   GhiChu = ct.GhiChu,
												   MaKhuyenMaiApDung = ct.MaKhuyenMaiApDung,
												   TenSanPham = ct.MaSanPhamNavigation.TenSanPham
											   })
											   .ToListAsync();

			if (chiTietDonHang == null)
			{
				return NotFound("Chi tiết đơn hàng không tìm thấy.");
			}

			return Ok(chiTietDonHang);
		}
		/// <summary>
		/// Lấy thông tin danh sách chi tiết của theo ID sản phẩm.
		/// </summary>
		[HttpGet("{maSanPham}")]
		public async Task<ActionResult<ChiTietDonHangDto>> GetByProductId(int maSanPham)
		{
			var chiTietDonHang = await _context.ChiTietDonHangs
											   .Include(ct => ct.MaSanPhamNavigation)
											   .Where(ct => ct.MaSanPham == maSanPham)
											   .Select(ct => new ChiTietDonHangDto
											   {
												   MaSanPham = ct.MaSanPham,
												   MaDonHang = ct.MaDonHang,
												   SoLuong = ct.SoLuong,
												   GhiChu = ct.GhiChu,
												   MaKhuyenMaiApDung = ct.MaKhuyenMaiApDung,
												   TenSanPham = ct.MaSanPhamNavigation.TenSanPham
											   })
											   .ToListAsync();

			if (chiTietDonHang == null)
			{
				return NotFound("Chi tiết đơn hàng không tìm thấy.");
			}

			return Ok(chiTietDonHang);
		}
		/// <summary>
		/// Tạo mới một chi tiết đơn hàng.
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<ChiTietDonHang>> CreateChiTietDonHang(ChiTietDonHangDto newChiTietDonHangDto)
		{
			if (newChiTietDonHangDto == null)
			{
				return BadRequest("Dữ liệu không hợp lệ.");
			}

			var newChiTietDonHang = new ChiTietDonHang
			{
				MaSanPham = newChiTietDonHangDto.MaSanPham,
				MaDonHang = newChiTietDonHangDto.MaDonHang,
				SoLuong = newChiTietDonHangDto.SoLuong,
				GhiChu = newChiTietDonHangDto.GhiChu,
				MaKhuyenMaiApDung = newChiTietDonHangDto.MaKhuyenMaiApDung
			};

			_context.ChiTietDonHangs.Add(newChiTietDonHang);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById),
								   new { maDonHang = newChiTietDonHang.MaDonHang, maSanPham = newChiTietDonHang.MaSanPham },
								   newChiTietDonHang);
		}

		/// <summary>
		/// Cập nhật thông tin chi tiết đơn hàng.
		/// </summary>
		[HttpPut("{maDonHang}/{maSanPham}")]
		public async Task<IActionResult> UpdateChiTietDonHang(int maDonHang, int maSanPham, ChiTietDonHangDto updatedDto)
		{
			if (updatedDto == null)
			{
				return BadRequest("Dữ liệu không hợp lệ.");
			}

			var existing = await _context.ChiTietDonHangs
										 .Where(ct => ct.MaDonHang == maDonHang && ct.MaSanPham == maSanPham)
										 .FirstOrDefaultAsync();

			if (existing == null)
			{
				return NotFound("Chi tiết đơn hàng không tìm thấy.");
			}

			existing.SoLuong = updatedDto.SoLuong;
			existing.GhiChu = updatedDto.GhiChu;
			existing.MaKhuyenMaiApDung = updatedDto.MaKhuyenMaiApDung;

			await _context.SaveChangesAsync();
			return NoContent();
		}

		/// <summary>
		/// Xóa một chi tiết đơn hàng (chuyển trạng thái An = true).
		/// </summary>
		[HttpDelete("{maDonHang}/{maSanPham}")]
		public async Task<IActionResult> DeleteChiTietDonHang(int maDonHang, int maSanPham)
		{
			var existing = await _context.ChiTietDonHangs
										 .Where(ct => ct.MaDonHang == maDonHang && ct.MaSanPham == maSanPham)
										 .FirstOrDefaultAsync();

			if (existing == null)
			{
				return NotFound("Chi tiết đơn hàng không tìm thấy.");
			}
			_context.Remove(existing);
			await _context.SaveChangesAsync();
			return NoContent();
		}

		/// <summary>
		/// Tìm kiếm chi tiết đơn hàng theo từ khóa.
		/// </summary>
		[HttpGet("{keyword}")]
		public async Task<ActionResult<IEnumerable<ChiTietDonHangDto>>> Search(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return BadRequest("Từ khóa tìm kiếm không hợp lệ.");
			}

			var results = await _context.ChiTietDonHangs
										.Include(ct => ct.MaSanPhamNavigation)
										.Where(ct => (ct.GhiChu.Contains(keyword) ||
													 ct.MaSanPhamNavigation.TenSanPham.Contains(keyword)))
										.Select(ct => new ChiTietDonHangDto
										{
											MaSanPham = ct.MaSanPham,
											MaDonHang = ct.MaDonHang,
											SoLuong = ct.SoLuong,
											GhiChu = ct.GhiChu,
											MaKhuyenMaiApDung = ct.MaKhuyenMaiApDung,
											TenSanPham = ct.MaSanPhamNavigation.TenSanPham
										})
										.ToListAsync();

			return Ok(results);
		}
	}
}
