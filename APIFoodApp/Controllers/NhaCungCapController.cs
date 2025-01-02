using APIFoodApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFoodApp.Controllers
{
	[Authorize(Roles ="Admin")]
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class NhaCungCapController : ControllerBase
	{
		private readonly ILogger<NhaCungCapController> _logger;
		private readonly FoodAppContext _context;

		public NhaCungCapController(ILogger<NhaCungCapController> logger, FoodAppContext context)
		{
			_logger = logger;
			_context = context;
		}

		/// <summary>
		/// Lấy danh sách tất cả các nhà cung cấp, bao gồm danh sách các sản phẩm thuộc về nhà cung cấp đó.
		/// </summary>
		/// <returns>Danh sách các nhà cung cấp.</returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<NhaCungCap>>> Get()
		{
			var suppliers = await _context.NhaCungCaps
										  .Include(ncc => ncc.SanPhams)
										  .Where(ncc => ncc.An == false)
										  .ToListAsync();
			return Ok(suppliers);
		}

		/// <summary>
		/// Lấy thông tin chi tiết của một nhà cung cấp dựa vào ID.
		/// </summary>
		/// <param name="id">ID của nhà cung cấp cần lấy.</param>
		/// <returns>Thông tin của nhà cung cấp nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<NhaCungCap>> GetById(int id)
		{
			var supplier = await _context.NhaCungCaps
										 .Include(ncc => ncc.SanPhams)
										 .Where(ncc => ncc.An == false)
										 .FirstOrDefaultAsync(ncc => ncc.MaNhaCungCap == id);

			if (supplier == null)
			{
				return NotFound("Supplier not found.");
			}

			return Ok(supplier);
		}

		/// <summary>
		/// Tạo mới một nhà cung cấp.
		/// </summary>
		/// <param name="newSupplier">Thông tin của nhà cung cấp mới cần tạo.</param>
		/// <returns>Nhà cung cấp vừa được tạo nếu thành công; nếu không, trả về thông báo lỗi.</returns>
		[HttpPost]
		public async Task<ActionResult<NhaCungCap>> CreateSupplier(NhaCungCap newSupplier)
		{
			if (newSupplier == null)
			{
				return BadRequest("Supplier data is null.");
			}
			newSupplier.An = false;
			_context.NhaCungCaps.Add(newSupplier);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById), new { id = newSupplier.MaNhaCungCap }, newSupplier);
		}

		/// <summary>
		/// Cập nhật thông tin của nhà cung cấp dựa vào ID.
		/// </summary>
		/// <param name="id">ID của nhà cung cấp cần cập nhật.</param>
		/// <param name="updatedSupplier">Thông tin mới của nhà cung cấp.</param>
		/// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateSupplier(int id, NhaCungCap updatedSupplier)
		{
			var existingSupplier = await _context.NhaCungCaps.FindAsync(id);
			if (existingSupplier == null)
			{
				return NotFound("Supplier not found.");
			}

			// Cập nhật các thuộc tính
			existingSupplier.TenNhaCungCap = updatedSupplier.TenNhaCungCap;
			existingSupplier.DiaChi = updatedSupplier.DiaChi;
			existingSupplier.SoDienThoai = updatedSupplier.SoDienThoai;
			existingSupplier.Email = updatedSupplier.Email;
			existingSupplier.An = updatedSupplier.An;
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await _context.NhaCungCaps.AnyAsync(ncc => ncc.MaNhaCungCap == id))
				{
					return NotFound("Supplier not found.");
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		/// <summary>
		/// Xóa một nhà cung cấp dựa vào ID.
		/// </summary>
		/// <param name="id">ID của nhà cung cấp cần xóa.</param>
		/// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteSupplier(int id)
		{
			var supplier = await _context.NhaCungCaps.FindAsync(id);
			if (supplier == null)
			{
				return NotFound("Supplier not found.");
			}

			supplier.An=true;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await _context.NhaCungCaps.AnyAsync(ncc => ncc.MaNhaCungCap == id))
				{
					return NotFound("Supplier not found.");
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		/// <summary>
		/// Tìm kiếm nhà cung cấp dựa trên từ khóa trong tên nhà cung cấp.
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (trong tên nhà cung cấp).</param>
		/// <returns>Danh sách các nhà cung cấp có chứa từ khóa trong tên.</returns>
		[HttpGet("{keyword}")]
		public async Task<ActionResult<IEnumerable<NhaCungCap>>> Search(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return BadRequest("Keyword cannot be empty.");
			}

			var searchResults = await _context.NhaCungCaps
											  .Where(ncc => ncc.TenNhaCungCap.Contains(keyword)
												|| ncc.MaNhaCungCap.ToString()==keyword.Trim() && ncc.An==false)
											  .ToListAsync();

			return Ok(searchResults);
		}
	}
}
