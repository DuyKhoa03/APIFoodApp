using APIFoodApp.Dtos;
using APIFoodApp.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFoodApp.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class SanPhamController : ControllerBase
	{
		private readonly ILogger<SanPhamController> _logger;
		private readonly FoodAppContext _context; 
		private readonly Cloudinary _cloudinary;

		public SanPhamController(ILogger<SanPhamController> logger, FoodAppContext context, Cloudinary cloudinary)
		{
			_logger = logger;
			_context = context;
			_cloudinary = cloudinary;
		}
		/// <summary>
		/// Lấy danh sách tất cả sản phẩm.
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<SanPhamDto>>> Get()
		{
			var products = await _context.SanPhams
										 .Where(sp => sp.An == false || sp.An == null)
										 .Include(sp => sp.MaLoaiNavigation)
										 .Include(sp => sp.MaNhaCungCapNavigation)
										 .Select(sp => new SanPhamDto
										 {
											 MaSanPham = sp.MaSanPham,
											 TenSanPham = sp.TenSanPham,
											 MoTa = sp.MoTa,
											 Gia = sp.Gia,
											 SoLuong = sp.SoLuong,
											 TrangThai = sp.TrangThai,
											 NgayTao = sp.NgayTao,
											 NgayCapNhat = sp.NgayCapNhat,
											 MaLoai = sp.MaLoai,
											 TenLoai = sp.MaLoaiNavigation.TenLoai,
											 MaNhaCungCap = sp.MaNhaCungCap,
											 TenNhaCungCap = sp.MaNhaCungCapNavigation.TenNhaCungCap,
											 Anh1 = sp.Anh1,
											 Anh2 = sp.Anh2,
											 Anh3 = sp.Anh3,
											 Anh4 = sp.Anh4,
											 Anh5 = sp.Anh5
										 })
										 .ToListAsync();

			return Ok(products);
		}

		/// <summary>
		/// Lấy thông tin chi tiết sản phẩm theo ID.
		/// </summary>
		[HttpGet("{id}")]
		public async Task<ActionResult<SanPhamDto>> GetById(int id)
		{
			var product = await _context.SanPhams
										.Where(sp => sp.MaSanPham == id && (sp.An == false || sp.An == null))
										.Include(sp => sp.MaLoaiNavigation)
										.Include(sp => sp.MaNhaCungCapNavigation)
										.Select(sp => new SanPhamDto
										{
											MaSanPham = sp.MaSanPham,
											TenSanPham = sp.TenSanPham,
											MoTa = sp.MoTa,
											Gia = sp.Gia,
											SoLuong = sp.SoLuong,
											TrangThai = sp.TrangThai,
											NgayTao = sp.NgayTao,
											NgayCapNhat = sp.NgayCapNhat,
											MaLoai = sp.MaLoai,
											TenLoai = sp.MaLoaiNavigation.TenLoai,
											MaNhaCungCap = sp.MaNhaCungCap,
											TenNhaCungCap = sp.MaNhaCungCapNavigation.TenNhaCungCap,
											Anh1 = sp.Anh1,
											Anh2 = sp.Anh2,
											Anh3 = sp.Anh3,
											Anh4 = sp.Anh4,
											Anh5 = sp.Anh5
										})
										.FirstOrDefaultAsync();

			if (product == null)
				return NotFound("Product not found.");

			return Ok(product);
		}

		/// <summary>
		/// Thêm sản phẩm mới.
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> Create([FromForm] SanPhamDto dto)
		{
			if (dto == null) return BadRequest("Product data is null.");

			var product = new SanPham
			{
				TenSanPham = dto.TenSanPham,
				MoTa = dto.MoTa,
				Gia = dto.Gia,
				SoLuong = dto.SoLuong,
				TrangThai = dto.TrangThai,
				MaLoai = dto.MaLoai,
				MaNhaCungCap = dto.MaNhaCungCap,
				NgayTao = DateTime.Now,
				An = false
			};

			// Xử lý upload ảnh lên Cloudinary
			var imageUrls = new List<string>();
			if (dto.Images != null && dto.Images.Any())
			{
				foreach (var img in dto.Images)
				{
					var uploadParams = new ImageUploadParams
					{
						File = new FileDescription(img.FileName, img.OpenReadStream()),
						Folder = "products", // Thư mục trên Cloudinary
						Transformation = new Transformation().Crop("limit").Width(800).Height(800)
					};

					var uploadResult = await _cloudinary.UploadAsync(uploadParams);
					if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
					{
						return BadRequest("Failed to upload image to Cloudinary.");
					}

					imageUrls.Add(uploadResult.SecureUrl.ToString());
				}

				// Gán ảnh vào sản phẩm
				product.Anh1 = imageUrls.ElementAtOrDefault(0);
				product.Anh2 = imageUrls.ElementAtOrDefault(1);
				product.Anh3 = imageUrls.ElementAtOrDefault(2);
				product.Anh4 = imageUrls.ElementAtOrDefault(3);
				product.Anh5 = imageUrls.ElementAtOrDefault(4);
			}
			else
			{
				// Nếu không có ảnh nào được tải lên
				product.Anh1 = "";
				product.Anh2 = "";
				product.Anh3 = "";
				product.Anh4 = "";
				product.Anh5 = "";
			}

			_context.SanPhams.Add(product);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById), new { id = product.MaSanPham }, product);
		}

		/// <summary>
		/// Cập nhật sản phẩm.
		/// </summary>
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromForm] SanPhamDto updatedProductDto)
		{
			if (updatedProductDto == null)
			{
				return BadRequest("Product data is null.");
			}

			var existingProduct = await _context.SanPhams.FindAsync(id);
			if (existingProduct == null)
			{
				return NotFound("Product not found.");
			}

			existingProduct.TenSanPham = updatedProductDto.TenSanPham;
			existingProduct.MoTa = updatedProductDto.MoTa;
			existingProduct.Gia = updatedProductDto.Gia;
			existingProduct.SoLuong = updatedProductDto.SoLuong;
			existingProduct.TrangThai = updatedProductDto.TrangThai;
			existingProduct.NgayCapNhat = DateTime.Now;
			existingProduct.MaLoai = updatedProductDto.MaLoai;
			existingProduct.MaNhaCungCap = updatedProductDto.MaNhaCungCap;
			// Xử lý danh sách ảnh tải lên (nếu có)
			if (updatedProductDto.Images != null && updatedProductDto.Images.Any())
			{
				var imageUrls = new List<string>();
				foreach (var img in updatedProductDto.Images)
				{
					var uploadParams = new ImageUploadParams
					{
						File = new FileDescription(img.FileName, img.OpenReadStream()),
						Folder = "products",
						Transformation = new Transformation().Crop("limit").Width(800).Height(800)
					};

					var uploadResult = await _cloudinary.UploadAsync(uploadParams);
					if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
					{
						return BadRequest("Failed to upload image to Cloudinary.");
					}

					imageUrls.Add(uploadResult.SecureUrl.ToString());
				}

				// Cập nhật các URL ảnh mới
				existingProduct.Anh1 = imageUrls.ElementAtOrDefault(0);
				existingProduct.Anh2 = imageUrls.ElementAtOrDefault(1);
				existingProduct.Anh3 = imageUrls.ElementAtOrDefault(2);
				existingProduct.Anh4 = imageUrls.ElementAtOrDefault(3);
				existingProduct.Anh5 = imageUrls.ElementAtOrDefault(4);
			}
			await _context.SaveChangesAsync();
			return NoContent();
		}

		/// <summary>
		/// Xóa mềm sản phẩm.
		/// </summary>
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var product = await _context.SanPhams.FindAsync(id);
			if (product == null) return NotFound("Product not found.");

			product.An = true;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		/// <summary>
		/// Tìm kiếm sản phẩm theo tên hoặc mô tả.
		/// </summary>
		[HttpGet("{keyword}")]
		public async Task<ActionResult<IEnumerable<SanPhamDto>>> Search(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword)) return BadRequest("Keyword cannot be empty.");

			var products = await _context.SanPhams
										 .Where(sp => (sp.An == false || sp.An == null) &&
													  (sp.TenSanPham.Contains(keyword) || sp.MoTa.Contains(keyword)))
										 .Select(sp => new SanPhamDto
										 {
											 MaSanPham = sp.MaSanPham,
											 TenSanPham = sp.TenSanPham,
											 MoTa = sp.MoTa,
											 Gia = sp.Gia,
											 SoLuong = sp.SoLuong,
											 TrangThai = sp.TrangThai,
											 NgayTao = sp.NgayTao,
											 NgayCapNhat = sp.NgayCapNhat
										 })
										 .ToListAsync();

			return Ok(products);
		}
	}
}
