using APIFoodApp.Dtos;
using APIFoodApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFoodApp.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class DiaChiController : ControllerBase
	{
		private readonly ILogger<DiaChiController> _logger;
		private readonly FoodAppContext _context;

		public DiaChiController(ILogger<DiaChiController> logger, FoodAppContext context)
		{
			_logger = logger;
			_context = context;
		}

		/// <summary>
		/// Lấy danh sách tất cả các địa chỉ.
		/// </summary>
		/// <returns>Danh sách các địa chỉ.</returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<DiaChiDto>>> Get()
		{
			var diaChis = await _context.DiaChis
										.Include(dc => dc.MaNguoiDungNavigation)
										.Where(dc=>dc.An == false)
										.Select(dc => new DiaChiDto
										{
											MaDiaChi = dc.MaDiaChi,
											MaNguoiDung = dc.MaNguoiDung,
											Ten = dc.Ten,
											DiaChi = dc.DiaChi1,
											TenNguoiDung = dc.MaNguoiDungNavigation.TenNguoiDung
										})
										.ToListAsync();

			return Ok(diaChis);
		}

		/// <summary>
		/// Lấy thông tin chi tiết của một địa chỉ theo ID.
		/// </summary>
		/// <param name="id">ID của địa chỉ cần lấy.</param>
		/// <returns>Thông tin của địa chỉ nếu tìm thấy.</returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<DiaChiDto>> GetById(int id)
		{
			var diaChi = await _context.DiaChis
									   .Include(dc => dc.MaNguoiDungNavigation)
									   .Where(dc => dc.MaDiaChi == id && dc.An == false)
									   .Select(dc => new DiaChiDto
									   {
										   MaDiaChi = dc.MaDiaChi,
										   MaNguoiDung = dc.MaNguoiDung,
										   Ten = dc.Ten,
										   DiaChi = dc.DiaChi1,
										   TenNguoiDung = dc.MaNguoiDungNavigation.TenNguoiDung
									   })
									   .FirstOrDefaultAsync();

			if (diaChi == null)
			{
				return NotFound("DiaChi not found.");
			}

			return Ok(diaChi);
		}

		/// <summary>
		/// Tạo mới một địa chỉ.
		/// </summary>
		/// <param name="newDiaChiDto">Thông tin địa chỉ mới cần tạo.</param>
		/// <returns>Địa chỉ vừa được tạo nếu thành công.</returns>
		[HttpPost]
		public async Task<ActionResult<DiaChi>> CreateDiaChi(DiaChiDto newDiaChiDto)
		{
			if (newDiaChiDto == null)
			{
				return BadRequest("DiaChi data is null.");
			}

			var newDiaChi = new DiaChi
			{
				MaNguoiDung = newDiaChiDto.MaNguoiDung,
				Ten = newDiaChiDto.Ten,
				DiaChi1 = newDiaChiDto.DiaChi,
				An = false
				
			};

			_context.DiaChis.Add(newDiaChi);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById), new { id = newDiaChi.MaDiaChi }, newDiaChi);
		}

		/// <summary>
		/// Cập nhật thông tin địa chỉ dựa vào ID.
		/// </summary>
		/// <param name="id">ID của địa chỉ cần cập nhật.</param>
		/// <param name="updatedDiaChiDto">Thông tin địa chỉ cần cập nhật.</param>
		/// <returns>Không trả về nội dung nếu cập nhật thành công.</returns>
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateDiaChi(int id, DiaChiDto updatedDiaChiDto)
		{
			if (updatedDiaChiDto == null)
			{
				return BadRequest("DiaChi data is null.");
			}

			var existingDiaChi = await _context.DiaChis.FindAsync(id);
			if (existingDiaChi == null)
			{
				return NotFound("DiaChi not found.");
			}

			// Cập nhật các thuộc tính
			existingDiaChi.MaNguoiDung = updatedDiaChiDto.MaNguoiDung;
			existingDiaChi.Ten = updatedDiaChiDto.Ten;
			existingDiaChi.DiaChi1 = updatedDiaChiDto.DiaChi;
			existingDiaChi.An = updatedDiaChiDto.An;
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await _context.DiaChis.AnyAsync(dc => dc.MaDiaChi == id))
				{
					return NotFound("DiaChi not found.");
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		/// <summary>
		/// Xóa một địa chỉ dựa vào ID.
		/// </summary>
		/// <param name="id">ID của địa chỉ cần xóa.</param>
		/// <returns>Không trả về nội dung nếu xóa thành công.</returns>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteDiaChi(int id)
		{
			var diaChi = await _context.DiaChis.FindAsync(id);
			if (diaChi == null)
			{
				return NotFound("DiaChi not found.");
			}

			diaChi.An = true;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		/// <summary>
		/// Tìm kiếm địa chỉ theo từ khóa.
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (tên địa chỉ hoặc tên người dùng).</param>
		/// <returns>Danh sách các địa chỉ phù hợp với từ khóa tìm kiếm.</returns>
		[HttpGet("{keyword}")]
		public async Task<ActionResult<IEnumerable<DiaChiDto>>> Search(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return BadRequest("Keyword cannot be empty.");
			}

			var searchResults = await _context.DiaChis
											  .Where(dc =>
												  dc.Ten.Contains(keyword) ||
												  dc.DiaChi1.Contains(keyword) ||
												  dc.MaDiaChi.ToString()==keyword.Trim()
												  && dc.An == false)
											  .Select(dc => new DiaChiDto
											  {
												  MaDiaChi = dc.MaDiaChi,
												  MaNguoiDung = dc.MaNguoiDung,
												  Ten = dc.Ten,
												  DiaChi = dc.DiaChi1,
												  TenNguoiDung = dc.MaNguoiDungNavigation.TenNguoiDung
											  })
											  .ToListAsync();

			return Ok(searchResults);
		}
	}
}
