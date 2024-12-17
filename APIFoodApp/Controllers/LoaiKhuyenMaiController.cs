using APIFoodApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFoodApp.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class LoaiKhuyenMaiController : ControllerBase
	{
		private readonly ILogger<LoaiKhuyenMaiController> _logger;
		private readonly FoodAppContext _context;

		public LoaiKhuyenMaiController(ILogger<LoaiKhuyenMaiController> logger, FoodAppContext context)
		{
			_logger = logger;
			_context = context;
		}

		/// <summary>
		/// Lấy danh sách tất cả các loại khuyến mãi, bao gồm danh sách khuyến mãi thuộc loại đó.
		/// </summary>
		/// <returns>Danh sách các loại khuyến mãi.</returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<LoaiKhuyenMai>>> Get()
		{
			var promoTypes = await _context.LoaiKhuyenMais
										   .Include(lkm => lkm.KhuyenMais)
										   .Where(lkm => lkm.An == false)
										   .ToListAsync();
			return Ok(promoTypes);
		}

		/// <summary>
		/// Lấy thông tin chi tiết của một loại khuyến mãi dựa vào ID.
		/// </summary>
		/// <param name="id">ID của loại khuyến mãi cần lấy.</param>
		/// <returns>Thông tin của loại khuyến mãi nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<LoaiKhuyenMai>> GetById(int id)
		{
			var promoType = await _context.LoaiKhuyenMais
										  .Include(lkm => lkm.KhuyenMais)
										  .Where(lkm => lkm.An == false)
										  .FirstOrDefaultAsync(lkm => lkm.MaLoai == id);

			if (promoType == null)
			{
				return NotFound("Promotion type not found.");
			}

			return Ok(promoType);
		}

		/// <summary>
		/// Tạo mới một loại khuyến mãi.
		/// </summary>
		/// <param name="newPromoType">Thông tin của loại khuyến mãi mới cần tạo.</param>
		/// <returns>Loại khuyến mãi vừa được tạo nếu thành công; nếu không, trả về thông báo lỗi.</returns>
		[HttpPost]
		public async Task<ActionResult<LoaiKhuyenMai>> CreatePromoType(LoaiKhuyenMai newPromoType)
		{
			if (newPromoType == null)
			{
				return BadRequest("Promotion type data is null.");
			}
			newPromoType.An = false;
			_context.LoaiKhuyenMais.Add(newPromoType);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById), new { id = newPromoType.MaLoai }, newPromoType);
		}

		/// <summary>
		/// Cập nhật thông tin của loại khuyến mãi dựa vào ID.
		/// </summary>
		/// <param name="id">ID của loại khuyến mãi cần cập nhật.</param>
		/// <param name="updatedPromoType">Thông tin mới của loại khuyến mãi.</param>
		/// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdatePromoType(int id, LoaiKhuyenMai updatedPromoType)
		{
			var existingPromoType = await _context.LoaiKhuyenMais.FindAsync(id);
			if (existingPromoType == null)
			{
				return NotFound("Promotion type not found.");
			}

			// Cập nhật các thuộc tính
			existingPromoType.TenLoai = updatedPromoType.TenLoai;
			existingPromoType.MoTa = updatedPromoType.MoTa;
			existingPromoType.An = updatedPromoType.An;
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await _context.LoaiKhuyenMais.AnyAsync(lkm => lkm.MaLoai == id))
				{
					return NotFound("Promotion type not found.");
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		/// <summary>
		/// Xóa một loại khuyến mãi dựa vào ID.
		/// </summary>
		/// <param name="id">ID của loại khuyến mãi cần xóa.</param>
		/// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePromoType(int id)
		{
			var promoType = await _context.LoaiKhuyenMais.FindAsync(id);
			if (promoType == null)
			{
				return NotFound("Promotion type not found.");
			}

			promoType.An = true;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await _context.LoaiKhuyenMais.AnyAsync(lkm => lkm.MaLoai == id))
				{
					return NotFound("Promotion type not found.");
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		/// <summary>
		/// Tìm kiếm loại khuyến mãi dựa trên từ khóa trong tên loại khuyến mãi.
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (trong tên loại khuyến mãi).</param>
		/// <returns>Danh sách các loại khuyến mãi có chứa từ khóa trong tên.</returns>
		[HttpGet("{keyword}")]
		public async Task<ActionResult<IEnumerable<LoaiKhuyenMai>>> Search(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return BadRequest("Keyword cannot be empty.");
			}

			var searchResults = await _context.LoaiKhuyenMais
											  .Where(lkm => lkm.TenLoai.Contains(keyword) || lkm.MaLoai.ToString()==keyword.Trim()
											  && lkm.An == false)
											  .ToListAsync();

			return Ok(searchResults);
		}
	}
}
