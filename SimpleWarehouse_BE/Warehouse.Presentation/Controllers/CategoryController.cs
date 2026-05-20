using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Warehouse.Application.DTO;
using Warehouse.Application.Interface.IService;
using Warehouse.Application.Middleware;
using Warehouse.Presentation.Middleware;

namespace Warehouse.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CategoryController : Controller
    {

        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet]
        [HasRole("Staff", "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [HasRole("Staff", "Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            if (result == null) return NotFound("Không tìm thấy danh mục yêu cầu.");
            return Ok(result);
        }

        [HttpPost]
        [HasRole("Staff", "Admin")]
        public async Task<IActionResult> Create([FromBody] CategoryRequest request)
        {
            var result = await _categoryService.CreateCategoryAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [HasRole("Staff", "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CategoryRequest request)
        {
            var success = await _categoryService.UpdateCategoryAsync(id, request);
            if (!success) return NotFound("Không tìm thấy danh mục cần cập nhật.");
            return Ok("Cập nhật danh mục thành công.");
        }

        // ĐẶC BIỆT: Xử lý logic từ chối xóa nếu có sản phẩm ràng buộc
        [HttpDelete("{id:guid}")]
        [HasRole("Staff", "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var status = await _categoryService.DeleteCategoryAsync(id);

            if (status == -1)
                return NotFound("Không tìm thấy danh mục cần xóa.");

            if (status > 0)
            {
                // Chửi khi có mặt hàng mà đòi xóa
                return BadRequest(new CategoryDeleteExceptionResponse
                {
                    Message = $"Không thể xóa danh mục này vì đang có {status} mặt hàng trực thuộc tồn tại trong hệ thống kho!",
                    AssociatedProductCount = status
                });
            }

            return Ok("Xóa danh mục thành công hoàn toàn.");
        }
    }
}
