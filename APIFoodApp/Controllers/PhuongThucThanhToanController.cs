using APIFoodApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFoodApp.Controllers
{
	[Authorize(Roles ="Admin")]
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class PhuongThucThanhToanController : ControllerBase
	{
		private readonly ILogger<PhuongThucThanhToanController> _logger;
		private readonly FoodAppContext _context;

		public PhuongThucThanhToanController(ILogger<PhuongThucThanhToanController> logger, FoodAppContext context)
		{
			_logger = logger;
			_context = context;
		}

		/// <summary>
		/// Lấy danh sách tất cả các phương thức thanh toán, bao gồm danh sách đơn hàng sử dụng phương thức đó.
		/// </summary>
		/// <returns>Danh sách các phương thức thanh toán.</returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<PhuongThucThanhToan>>> Get()
		{
			var paymentMethods = await _context.PhuongThucThanhToans
											   .Include(pt => pt.DonHangs)
											   .Where(pt=>pt.An == false)
											   .ToListAsync();
			return Ok(paymentMethods);
		}

		/// <summary>
		/// Lấy thông tin chi tiết của một phương thức thanh toán dựa vào ID.
		/// </summary>
		/// <param name="id">ID của phương thức thanh toán cần lấy.</param>
		/// <returns>Thông tin của phương thức thanh toán nếu tìm thấy; nếu không, trả về thông báo lỗi.</returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<PhuongThucThanhToan>> GetById(int id)
		{
			var paymentMethod = await _context.PhuongThucThanhToans
											  .Include(pt => pt.DonHangs)
											  .Where(pt => pt.An == false)
											  .FirstOrDefaultAsync(pt => pt.MaPhuongThuc == id);

			if (paymentMethod == null)
			{
				return NotFound("Payment method not found.");
			}

			return Ok(paymentMethod);
		}

		/// <summary>
		/// Tạo mới một phương thức thanh toán.
		/// </summary>
		/// <param name="newPaymentMethod">Thông tin của phương thức thanh toán mới cần tạo.</param>
		/// <returns>Phương thức thanh toán vừa được tạo nếu thành công; nếu không, trả về thông báo lỗi.</returns>
		[HttpPost]
		public async Task<ActionResult<PhuongThucThanhToan>> CreatePaymentMethod(PhuongThucThanhToan newPaymentMethod)
		{
			if (newPaymentMethod == null)
			{
				return BadRequest("Payment method data is null.");
			}
			newPaymentMethod.An = false;
			_context.PhuongThucThanhToans.Add(newPaymentMethod);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById), new { id = newPaymentMethod.MaPhuongThuc }, newPaymentMethod);
		}

		/// <summary>
		/// Cập nhật thông tin của phương thức thanh toán dựa vào ID.
		/// </summary>
		/// <param name="id">ID của phương thức thanh toán cần cập nhật.</param>
		/// <param name="updatedPaymentMethod">Thông tin mới của phương thức thanh toán.</param>
		/// <returns>Không trả về nội dung nếu cập nhật thành công; nếu không, trả về thông báo lỗi.</returns>
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdatePaymentMethod(int id, PhuongThucThanhToan updatedPaymentMethod)
		{
			var existingPaymentMethod = await _context.PhuongThucThanhToans.FindAsync(id);
			if (existingPaymentMethod == null)
			{
				return NotFound("Payment method not found.");
			}

			// Cập nhật các thuộc tính
			existingPaymentMethod.Ten = updatedPaymentMethod.Ten;
			existingPaymentMethod.MoTa = updatedPaymentMethod.MoTa;
			existingPaymentMethod.An = updatedPaymentMethod.An;
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await _context.PhuongThucThanhToans.AnyAsync(pt => pt.MaPhuongThuc == id))
				{
					return NotFound("Payment method not found.");
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		/// <summary>
		/// Xóa một phương thức thanh toán dựa vào ID.
		/// </summary>
		/// <param name="id">ID của phương thức thanh toán cần xóa.</param>
		/// <returns>Không trả về nội dung nếu xóa thành công; nếu không, trả về thông báo lỗi.</returns>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePaymentMethod(int id)
		{
			var paymentMethod = await _context.PhuongThucThanhToans.FindAsync(id);
			if (paymentMethod == null)
			{
				return NotFound("Payment method not found.");
			}

			paymentMethod.An = true;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await _context.PhuongThucThanhToans.AnyAsync(pt => pt.MaPhuongThuc == id))
				{
					return NotFound("Payment method not found.");
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		/// <summary>
		/// Tìm kiếm phương thức thanh toán dựa trên từ khóa trong tên phương thức thanh toán.
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (trong tên phương thức thanh toán).</param>
		/// <returns>Danh sách các phương thức thanh toán có chứa từ khóa trong tên.</returns>
		[HttpGet("{keyword}")]
		public async Task<ActionResult<IEnumerable<PhuongThucThanhToan>>> Search(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return BadRequest("Keyword cannot be empty.");
			}

			var searchResults = await _context.PhuongThucThanhToans
											  .Where(pt => pt.Ten.Contains(keyword)
											  || pt.MaPhuongThuc.ToString()==keyword.Trim() && pt.An == false)
											  .ToListAsync();

			return Ok(searchResults);
		}
	}
}
