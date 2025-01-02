using APIFoodApp.Dtos;
using APIFoodApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFoodApp.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class LichSuDonHangController : ControllerBase
	{
		private readonly ILogger<LichSuDonHangController> _logger;
		private readonly FoodAppContext _context;

		public LichSuDonHangController(ILogger<LichSuDonHangController> logger, FoodAppContext context)
		{
			_logger = logger;
			_context = context;
		}

		/// <summary>
		/// Lấy danh sách tất cả các LichSuDonHang.
		/// </summary>
		/// <returns>Danh sách các LichSuDonHang.</returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<LichSuDonHangDto>>> Get()
		{
			var lichSuDonHangs = await _context.LichSuDonHangs
										.Where(ls=>ls.An == false)
									  .Select(ls => new LichSuDonHangDto
									  {
										  MaLichSu = ls.MaLichSu,
										  MaDonHang = ls.MaDonHang,
										  NgayTao = ls.NgayTao,
										  TrangThai = ls.TrangThai,
										  GhiChu = ls.GhiChu
									  })
									  .ToListAsync();

			return Ok(lichSuDonHangs);
		}

		/// <summary>
		/// Lấy thông tin chi tiết của một LichSuDonHang theo ID.
		/// </summary>
		/// <param name="id">ID của LichSuDonHang cần lấy.</param>
		/// <returns>Thông tin của LichSuDonHang nếu tìm thấy.</returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<LichSuDonHangDto>> GetById(int id)
		{
			var lichSuDonHang = await _context.LichSuDonHangs
									 .Where(ls => ls.MaLichSu == id && ls.An == false)
									 .Select(ls => new LichSuDonHangDto
									 {
										 MaLichSu = ls.MaLichSu,
										 MaDonHang = ls.MaDonHang,
										 NgayTao = ls.NgayTao,
										 TrangThai = ls.TrangThai,
										 GhiChu = ls.GhiChu
									 })
									 .FirstOrDefaultAsync();

			if (lichSuDonHang == null)
			{
				return NotFound("LichSuDonHang not found.");
			}

			return Ok(lichSuDonHang);
		}

		/// <summary>
		/// Tạo mới một LichSuDonHang.
		/// </summary>
		/// <param name="newLichSuDonHangDto">Thông tin LichSuDonHang mới cần tạo.</param>
		/// <returns>LichSuDonHang vừa được tạo nếu thành công.</returns>
		[HttpPost]
		public async Task<ActionResult<LichSuDonHang>> CreateLichSuDonHang(LichSuDonHangDto newLichSuDonHangDto)
		{
			if (newLichSuDonHangDto == null)
			{
				return BadRequest("LichSuDonHang data is null.");
			}

			var newLichSuDonHang = new LichSuDonHang
			{
				MaDonHang = newLichSuDonHangDto.MaDonHang,
				NgayTao = newLichSuDonHangDto.NgayTao,
				TrangThai = newLichSuDonHangDto.TrangThai,
				GhiChu = newLichSuDonHangDto.GhiChu,
				An = false
			};

			_context.LichSuDonHangs.Add(newLichSuDonHang);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById), new { id = newLichSuDonHang.MaLichSu }, newLichSuDonHang);
		}

		/// <summary>
		/// Cập nhật thông tin của LichSuDonHang dựa vào ID.
		/// </summary>
		/// <param name="id">ID của LichSuDonHang cần cập nhật.</param>
		/// <param name="updatedLichSuDonHangDto">Thông tin LichSuDonHang cần cập nhật.</param>
		/// <returns>Không trả về nội dung nếu cập nhật thành công.</returns>
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateLichSuDonHang(int id, LichSuDonHangDto updatedLichSuDonHangDto)
		{
			if (updatedLichSuDonHangDto == null)
			{
				return BadRequest("LichSuDonHang data is null.");
			}

			var existingLichSuDonHang = await _context.LichSuDonHangs.FindAsync(id);
			if (existingLichSuDonHang == null)
			{
				return NotFound("LichSuDonHang not found.");
			}

			// Cập nhật các thuộc tính
			existingLichSuDonHang.MaDonHang = updatedLichSuDonHangDto.MaDonHang;
			existingLichSuDonHang.NgayTao = updatedLichSuDonHangDto.NgayTao;
			existingLichSuDonHang.TrangThai = updatedLichSuDonHangDto.TrangThai;
			existingLichSuDonHang.GhiChu = updatedLichSuDonHangDto.GhiChu;
			existingLichSuDonHang.An = updatedLichSuDonHangDto.An;
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await _context.LichSuDonHangs.AnyAsync(ls => ls.MaLichSu == id))
				{
					return NotFound("LichSuDonHang not found.");
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		/// <summary>
		/// Xóa một LichSuDonHang dựa vào ID.
		/// </summary>
		/// <param name="id">ID của LichSuDonHang cần xóa.</param>
		/// <returns>Không trả về nội dung nếu xóa thành công.</returns>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteLichSuDonHang(int id)
		{
			var lichSuDonHang = await _context.LichSuDonHangs.FindAsync(id);
			if (lichSuDonHang == null)
			{
				return NotFound("LichSuDonHang not found.");
			}

			lichSuDonHang.An = true;
			await _context.SaveChangesAsync();

			return NoContent();
		}
		[HttpGet("{keyword}")]
		public async Task<ActionResult<IEnumerable<LichSuDonHangDto>>> Search(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return BadRequest("Keyword cannot be empty.");
			}

			var searchResults = await _context.LichSuDonHangs
											  .Include(m => m.MaDonHang)
											  .Where(m => m.MaLichSu.ToString().Contains(keyword) && m.An == false)
											  .ToListAsync();

			return Ok(searchResults);
		}
	}
}
