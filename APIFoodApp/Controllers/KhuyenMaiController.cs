using APIFoodApp.Dtos;
using APIFoodApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFoodApp.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class KhuyenMaiController : ControllerBase
	{
		private readonly ILogger<KhuyenMaiController> _logger;
		private readonly FoodAppContext _context;

		public KhuyenMaiController(ILogger<KhuyenMaiController> logger, FoodAppContext context)
		{
			_logger = logger;
			_context = context;
		}

		/// <summary>
		/// Lấy danh sách tất cả các KhuyenMai, chỉ bao gồm các KhuyenMai không bị ẩn.
		/// </summary>
		/// <returns>Danh sách các KhuyenMai sắp xếp theo thứ tự.</returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<KhuyenMaiDto>>> Get()
		{
			var KhuyenMais = await _context.KhuyenMais
									  .Where(m => m.An == false)
									  .Select(m => new KhuyenMaiDto
									  {
										  MaKhuyenMai = m.MaKhuyenMai,
										  MaLoai = m.MaLoai,
										  Ten = m.Ten,
										  GiaTri = m.GiaTri,
										  DieuKienApDung = m.DieuKienApDung,
										  BatDau = m.BatDau,
										  KetThuc = m.KetThuc,
										  NgayTao = m.NgayTao,
										  NgayCapNhat = m.NgayCapNhat,
										  TenLoai = m.MaLoaiNavigation.TenLoai
									  })
									  .ToListAsync();

			return Ok(KhuyenMais);
		}

		/// <summary>
		/// Lấy thông tin chi tiết của một KhuyenMai theo ID.
		/// </summary>
		/// <param name="id">ID của KhuyenMai cần lấy.</param>
		/// <returns>Thông tin của KhuyenMai nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<KhuyenMaiDto>> GetById(int id)
		{
			var KhuyenMai = await _context.KhuyenMais
									 .Where(m => m.MaKhuyenMai == id && m.An == false)
									 .Select(m => new KhuyenMaiDto
									 {
										 MaKhuyenMai = m.MaKhuyenMai,
										 MaLoai = m.MaLoai,
										 Ten = m.Ten,
										 GiaTri = m.GiaTri,
										 DieuKienApDung = m.DieuKienApDung,
										 BatDau = m.BatDau,
										 KetThuc = m.KetThuc,
										 NgayTao = m.NgayTao,
										 NgayCapNhat = m.NgayCapNhat,
										 TenLoai = m.MaLoaiNavigation.TenLoai
									 })
									 .FirstOrDefaultAsync();

			if (KhuyenMai == null)
			{
				return NotFound("KhuyenMai not found.");
			}

			return Ok(KhuyenMai);
		}

		[HttpGet("{name}")]
		public async Task<ActionResult<KhuyenMaiDto>> GetByName(string name)
		{
			var KhuyenMai = await _context.KhuyenMais
									 .Where(m => m.Ten == name && m.An == false)
									 .Select(m => new KhuyenMaiDto
									 {
										 MaKhuyenMai = m.MaKhuyenMai,
										 MaLoai = m.MaLoai,
										 Ten = m.Ten,
										 GiaTri = m.GiaTri,
										 DieuKienApDung = m.DieuKienApDung,
										 BatDau = m.BatDau,
										 KetThuc = m.KetThuc,
										 NgayTao = m.NgayTao,
										 NgayCapNhat = m.NgayCapNhat,
										 TenLoai = m.MaLoaiNavigation.TenLoai
									 })
									 .FirstOrDefaultAsync();

			if (KhuyenMai == null)
			{
				return NotFound("KhuyenMai not found.");
			}

			return Ok(KhuyenMai);
		}

		/// <summary>
		/// Tạo mới một KhuyenMai.
		/// </summary>
		/// <param name="newKhuyenMaiDto">Thông tin KhuyenMai mới cần tạo.</param>
		/// <returns>KhuyenMai vừa được tạo nếu thành công; nếu không, trả về thông báo lỗi.</returns>
		[HttpPost]
		public async Task<ActionResult<KhuyenMai>> CreateKhuyenMai(KhuyenMaiDto newKhuyenMaiDto)
		{
			if (newKhuyenMaiDto == null)
			{
				return BadRequest("KhuyenMai data is null.");
			}

			var newKhuyenMai = new KhuyenMai
			{
				Ten = newKhuyenMaiDto.Ten,
				GiaTri = newKhuyenMaiDto.GiaTri,
				DieuKienApDung = newKhuyenMaiDto.DieuKienApDung,
				BatDau = newKhuyenMaiDto.BatDau,
				KetThuc = newKhuyenMaiDto.KetThuc,
				NgayTao = newKhuyenMaiDto.NgayTao,
				NgayCapNhat = newKhuyenMaiDto.NgayCapNhat,
				MaLoai = newKhuyenMaiDto.MaLoai,
				An = false
			};

			_context.KhuyenMais.Add(newKhuyenMai);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById), new { id = newKhuyenMai.MaKhuyenMai }, newKhuyenMai);
		}

		/// <summary>
		/// Cập nhật thông tin của KhuyenMai dựa vào ID.
		/// </summary>
		/// <param name="id">ID của KhuyenMai cần cập nhật.</param>
		/// <param name="updatedKhuyenMaiDto">Thông tin KhuyenMai cần cập nhật.</param>
		/// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateKhuyenMai(int id, KhuyenMaiDto updatedKhuyenMaiDto)
		{
			if (updatedKhuyenMaiDto == null)
			{
				return BadRequest("KhuyenMai data is null.");
			}

			var existingKhuyenMai = await _context.KhuyenMais.FindAsync(id);
			if (existingKhuyenMai == null)
			{
				return NotFound("KhuyenMai not found.");
			}

			// Cập nhật các thuộc tính
			existingKhuyenMai.Ten = updatedKhuyenMaiDto.Ten;
			existingKhuyenMai.GiaTri = updatedKhuyenMaiDto.GiaTri;
			existingKhuyenMai.DieuKienApDung = updatedKhuyenMaiDto.DieuKienApDung;
			existingKhuyenMai.BatDau = updatedKhuyenMaiDto.BatDau;
			existingKhuyenMai.KetThuc = updatedKhuyenMaiDto.KetThuc;
			existingKhuyenMai.NgayTao = updatedKhuyenMaiDto.NgayTao;
			existingKhuyenMai.NgayCapNhat = updatedKhuyenMaiDto.NgayCapNhat;
			existingKhuyenMai.MaLoai = updatedKhuyenMaiDto.MaLoai;
			existingKhuyenMai.An = updatedKhuyenMaiDto.An;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await _context.KhuyenMais.AnyAsync(m => m.MaKhuyenMai == id))
				{
					return NotFound("KhuyenMai not found.");
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		/// <summary>
		/// Xóa (ẩn) một KhuyenMai dựa vào ID.
		/// </summary>
		/// <param name="id">ID của KhuyenMai cần xóa.</param>
		/// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteKhuyenMai(int id)
		{
			var KhuyenMai = await _context.KhuyenMais.FindAsync(id);
			if (KhuyenMai == null)
			{
				return NotFound("KhuyenMai not found.");
			}

			// Ẩn KhuyenMai thay vì xóa vĩnh viễn
			KhuyenMai.An = true;
			await _context.SaveChangesAsync();

			return NoContent();
		}
		/// <summary>
		/// Tìm kiếm KhuyenMai theo tên.
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (tên KhuyenMai).</param>
		/// <returns>Danh sách các KhuyenMai có tên phù hợp với từ khóa tìm kiếm.</returns>
		// GET: api/KhuyenMai/search/{keyword}
		[HttpGet("{keyword}")]
		public async Task<ActionResult<IEnumerable<KhuyenMai>>> Search(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return BadRequest("Keyword cannot be empty.");
			}

			var searchResults = await _context.KhuyenMais
											  .Include(m => m.MaLoai)
											  .Where(m => m.Ten.Contains(keyword) || m.MaKhuyenMai.ToString()  == keyword.Trim()  && m.An == false)
											  .ToListAsync();

			return Ok(searchResults);
		}
	}
}
