using APIFoodApp.Dtos;
using APIFoodApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFoodApp.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class DonHangController : ControllerBase
	{
		private readonly ILogger<DonHangController> _logger;
		private readonly FoodAppContext _context;

		public DonHangController(ILogger<DonHangController> logger, FoodAppContext context)
		{
			_logger = logger;
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<DonHangDto>>> Get()
		{
			var donHangs = await _context.DonHangs
									   .Include(dh => dh.MaNguoiDungNavigation)
									   .Include(dh => dh.MaPhuongThucNavigation)
									   .Include(dh => dh.MaDiaChi)
									   .Where(dh => dh.An == false)
									   .Select(dh => new DonHangDto
									   {
										   MaDonHang = dh.MaDonHang,
										   MaNguoiDung = dh.MaNguoiDung,
										   MaPhuongThuc = dh.MaPhuongThuc,
										   MaDiaChi = dh.MaDiaChi,
										   TongTien = dh.TongTien,
										   TrangThai = dh.TrangThai,
										   NgayTao = dh.NgayTao,
										   NgayCapNhat = dh.NgayCapNhat,
										   An = dh.An,
										   TenNguoiDung = dh.MaNguoiDungNavigation.TenNguoiDung,
										   TenPhuongThuc = dh.MaPhuongThucNavigation.Ten,
										   TenDiaChi = dh.MaDiaChiNavigation.Ten,
									   })
									   .ToListAsync();

			return Ok(donHangs);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<DonHangDto>> GetById(int id)
		{
			var donHang = await _context.DonHangs
								   .Include(dh => dh.MaNguoiDungNavigation)
								   .Include(dh => dh.MaPhuongThucNavigation)
								   .Include(dh => dh.MaDiaChi)
								   .Where(dh => dh.MaDonHang == id && dh.An == false)
								   .Select(dh => new DonHangDto
								   {
									   MaDonHang = dh.MaDonHang,
									   MaNguoiDung = dh.MaNguoiDung,
									   MaPhuongThuc = dh.MaPhuongThuc,
									   TongTien = dh.TongTien,
									   TrangThai = dh.TrangThai,
									   NgayTao = dh.NgayTao,
									   NgayCapNhat = dh.NgayCapNhat,
									   An = dh.An,
									   TenNguoiDung = dh.MaNguoiDungNavigation.TenNguoiDung,
									   TenPhuongThuc = dh.MaPhuongThucNavigation.Ten,
									   TenDiaChi = dh.MaDiaChiNavigation.Ten,
								   })
								   .FirstOrDefaultAsync();

			if (donHang == null)
			{
				return NotFound("DonHang not found.");
			}

			return Ok(donHang);
		}

		[HttpPost]
		public async Task<ActionResult<DonHang>> CreateDonHang(DonHangDto newDonHangDto)
		{
			if (newDonHangDto == null)
			{
				return BadRequest("DonHang data is null.");
			}

			var newDonHang = new DonHang
			{
				MaNguoiDung = newDonHangDto.MaNguoiDung,
				MaPhuongThuc = newDonHangDto.MaPhuongThuc,
				MaDiaChi = newDonHangDto.MaDiaChi,
				TongTien = newDonHangDto.TongTien,
				TrangThai = newDonHangDto.TrangThai,
				NgayTao = newDonHangDto.NgayTao,
				NgayCapNhat = newDonHangDto.NgayCapNhat,
				An = false
			};

			_context.DonHangs.Add(newDonHang);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById), new { id = newDonHang.MaDonHang }, newDonHang);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateDonHang(int id, DonHangDto updatedDonHangDto)
		{
			if (updatedDonHangDto == null)
			{
				return BadRequest("DonHang data is null.");
			}

			var existingDonHang = await _context.DonHangs.FindAsync(id);
			if (existingDonHang == null)
			{
				return NotFound("DonHang not found.");
			}

			existingDonHang.MaNguoiDung = updatedDonHangDto.MaNguoiDung;
			existingDonHang.MaPhuongThuc = updatedDonHangDto.MaPhuongThuc;
			existingDonHang.MaDiaChi = updatedDonHangDto.MaDiaChi;
			existingDonHang.TongTien = updatedDonHangDto.TongTien;
			existingDonHang.TrangThai = updatedDonHangDto.TrangThai;
			existingDonHang.NgayTao = updatedDonHangDto.NgayTao;
			existingDonHang.NgayCapNhat = updatedDonHangDto.NgayCapNhat;
			existingDonHang.An = updatedDonHangDto.An ?? false;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await _context.DonHangs.AnyAsync(dh => dh.MaDonHang == id))
				{
					return NotFound("DonHang not found.");
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteDonHang(int id)
		{
			var donHang = await _context.DonHangs.FindAsync(id);
			if (donHang == null)
			{
				return NotFound("DonHang not found.");
			}

			donHang.An = true;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		[HttpGet("{keyword}")]
		public async Task<ActionResult<IEnumerable<DonHangDto>>> Search(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return BadRequest("Keyword cannot be empty.");
			}

			var searchResults = await _context.DonHangs
											  .Include(dh => dh.MaNguoiDungNavigation)
											  .Include(dh => dh.MaPhuongThucNavigation)
											  .Include(dh => dh.MaDiaChiNavigation)
											  .Where(dh =>
												  (dh.MaDonHang.ToString().Contains(keyword))
												  && dh.An == false)
											  .Select(dh => new DonHangDto
											  {
												  MaDonHang = dh.MaDonHang,
												  MaNguoiDung = dh.MaNguoiDung,
												  MaPhuongThuc = dh.MaPhuongThuc,
												  MaDiaChi = dh.MaDiaChi,
												  TongTien = dh.TongTien,
												  TrangThai = dh.TrangThai,
												  NgayTao = dh.NgayTao,
												  NgayCapNhat = dh.NgayCapNhat,
												  An = dh.An,
												  TenNguoiDung = dh.MaNguoiDungNavigation.TenNguoiDung,
												  TenPhuongThuc = dh.MaPhuongThucNavigation.Ten,
												  TenDiaChi = dh.MaDiaChiNavigation.Ten
											  })
											  .ToListAsync();

			return Ok(searchResults);
		}
	}
}
