using APIFoodApp.Dtos;
using APIFoodApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFoodApp.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class ThongBaoController : ControllerBase
	{
		private readonly ILogger<ThongBaoController> _logger;
		private readonly FoodAppContext _context;

		public ThongBaoController(ILogger<ThongBaoController> logger, FoodAppContext context)
		{
			_logger = logger;
			_context = context;
		}

		/// <summary>
		/// Lấy danh sách tất cả các thông báo, chỉ bao gồm các thông báo không bị ẩn.
		/// </summary>
		/// <returns>Danh sách các thông báo.</returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ThongBaoDto>>> Get()
		{
			var thongBaos = await _context.ThongBaos
										  .Include(tb => tb.MaNguoiDungNavigation)
										  .Where(tb => tb.An == false)
										  .Select(tb => new ThongBaoDto
										  {
											  MaThongBao = tb.MaThongBao,
											  MaNguoiDung = tb.MaNguoiDung,
											  Ten = tb.Ten,
											  MoTa = tb.MoTa,
											  NoiDung = tb.NoiDung,
											  TheLoai = tb.TheLoai,
											  DieuKienKichHoat = tb.DieuKienKichHoat,
											  An = tb.An,
											  NgayTao = tb.NgayTao,
											  NgayCapNhat = tb.NgayCapNhat,
											  TenNguoiDung = tb.MaNguoiDungNavigation.TenNguoiDung
										  })
										  .ToListAsync();

			return Ok(thongBaos);
		}

		/// <summary>
		/// Lấy thông tin chi tiết của một thông báo theo ID.
		/// </summary>
		/// <param name="id">ID của thông báo cần lấy.</param>
		/// <returns>Thông tin của thông báo nếu tìm thấy.</returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<ThongBaoDto>> GetById(int id)
		{
			var thongBao = await _context.ThongBaos
										 .Include(tb => tb.MaNguoiDungNavigation)
										 .Where(tb => tb.MaThongBao == id && tb.An == false)
										 .Select(tb => new ThongBaoDto
										 {
											 MaThongBao = tb.MaThongBao,
											 MaNguoiDung = tb.MaNguoiDung,
											 Ten = tb.Ten,
											 MoTa = tb.MoTa,
											 NoiDung = tb.NoiDung,
											 TheLoai = tb.TheLoai,
											 DieuKienKichHoat = tb.DieuKienKichHoat,
											 An = tb.An,
											 NgayTao = tb.NgayTao,
											 NgayCapNhat = tb.NgayCapNhat,
											 TenNguoiDung = tb.MaNguoiDungNavigation.TenNguoiDung
										 })
										 .FirstOrDefaultAsync();

			if (thongBao == null)
			{
				return NotFound("ThongBao not found.");
			}

			return Ok(thongBao);
		}

		/// <summary>
		/// Tạo mới một thông báo.
		/// </summary>
		/// <param name="newThongBaoDto">Thông tin thông báo mới cần tạo.</param>
		/// <returns>Thông báo vừa được tạo nếu thành công.</returns>
		[HttpPost]
		public async Task<ActionResult<ThongBao>> CreateThongBao(ThongBaoDto newThongBaoDto)
		{
			if (newThongBaoDto == null)
			{
				return BadRequest("ThongBao data is null.");
			}

			var newThongBao = new ThongBao
			{
				MaNguoiDung = newThongBaoDto.MaNguoiDung,
				Ten = newThongBaoDto.Ten,
				MoTa = newThongBaoDto.MoTa,
				NoiDung = newThongBaoDto.NoiDung,
				TheLoai = newThongBaoDto.TheLoai,
				DieuKienKichHoat = newThongBaoDto.DieuKienKichHoat,
				An = false,
				NgayTao = newThongBaoDto.NgayTao,
				NgayCapNhat = newThongBaoDto.NgayCapNhat
			};

			_context.ThongBaos.Add(newThongBao);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById), new { id = newThongBao.MaThongBao }, newThongBao);
		}

		/// <summary>
		/// Cập nhật thông tin của thông báo dựa vào ID.
		/// </summary>
		/// <param name="id">ID của thông báo cần cập nhật.</param>
		/// <param name="updatedThongBaoDto">Thông tin thông báo cần cập nhật.</param>
		/// <returns>Không trả về nội dung nếu cập nhật thành công.</returns>
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateThongBao(int id, ThongBaoDto updatedThongBaoDto)
		{
			if (updatedThongBaoDto == null)
			{
				return BadRequest("ThongBao data is null.");
			}

			var existingThongBao = await _context.ThongBaos.FindAsync(id);
			if (existingThongBao == null)
			{
				return NotFound("ThongBao not found.");
			}

			// Cập nhật các thuộc tính
			existingThongBao.MaNguoiDung = updatedThongBaoDto.MaNguoiDung;
			existingThongBao.Ten = updatedThongBaoDto.Ten;
			existingThongBao.MoTa = updatedThongBaoDto.MoTa;
			existingThongBao.NoiDung = updatedThongBaoDto.NoiDung;
			existingThongBao.TheLoai = updatedThongBaoDto.TheLoai;
			existingThongBao.DieuKienKichHoat = updatedThongBaoDto.DieuKienKichHoat;
			existingThongBao.An = updatedThongBaoDto.An ?? false;
			existingThongBao.NgayTao = updatedThongBaoDto.NgayTao;
			existingThongBao.NgayCapNhat = updatedThongBaoDto.NgayCapNhat;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await _context.ThongBaos.AnyAsync(tb => tb.MaThongBao == id))
				{
					return NotFound("ThongBao not found.");
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		/// <summary>
		/// Xóa (ẩn) một thông báo dựa vào ID.
		/// </summary>
		/// <param name="id">ID của thông báo cần xóa.</param>
		/// <returns>Không trả về nội dung nếu xóa thành công.</returns>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteThongBao(int id)
		{
			var thongBao = await _context.ThongBaos.FindAsync(id);
			if (thongBao == null)
			{
				return NotFound("ThongBao not found.");
			}

			// Ẩn thông báo thay vì xóa vĩnh viễn
			thongBao.An = true;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		/// <summary>
		/// Tìm kiếm thông báo theo từ khóa.
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (tên, mô tả, nội dung, hoặc thể loại).</param>
		/// <returns>Danh sách các thông báo phù hợp với từ khóa tìm kiếm.</returns>
		[HttpGet("{keyword}")]
		public async Task<ActionResult<IEnumerable<ThongBaoDto>>> Search(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return BadRequest("Keyword cannot be empty.");
			}

			var searchResults = await _context.ThongBaos
											  .Include(tb => tb.MaNguoiDungNavigation)
											  .Where(tb =>
												  tb.Ten.Contains(keyword) ||
												  tb.MoTa.Contains(keyword) ||
												  tb.NoiDung.Contains(keyword) ||
												  tb.TheLoai.Contains(keyword) &&
												  tb.An == false)
											  .Select(tb => new ThongBaoDto
											  {
												  MaThongBao = tb.MaThongBao,
												  MaNguoiDung = tb.MaNguoiDung,
												  Ten = tb.Ten,
												  MoTa = tb.MoTa,
												  NoiDung = tb.NoiDung,
												  TheLoai = tb.TheLoai,
												  DieuKienKichHoat = tb.DieuKienKichHoat,
												  An = tb.An,
												  NgayTao = tb.NgayTao,
												  NgayCapNhat = tb.NgayCapNhat,
												  TenNguoiDung = tb.MaNguoiDungNavigation.TenNguoiDung
											  })
											  .ToListAsync();

			return Ok(searchResults);
		}
	}
}
