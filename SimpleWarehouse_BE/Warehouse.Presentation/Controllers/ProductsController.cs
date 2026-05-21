using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Warehouse.Application.DTO;
using Warehouse.Application.Interface.IService;
using Warehouse.Application.Middleware;
using Warehouse.Domain.Entities;
using Warehouse.Presentation.Controllers;
using Warehouse.Presentation.Middleware;

namespace Warehouse.Presentation.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // Endpoint: GET /api/products
        [HttpGet]
        [HasRole("Staff", "Admin")]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productService.GetAllProductsAsync();
            return Ok(result);
        }
        // Endpoint: GET /api/products/{id}
        [HttpGet("{id}")]
        [HasRole("Staff", "Admin")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (result == null) return NotFound("sản phẩm không tồn tại");
            return Ok(result);
        }
        [HttpPost]
        [HasRole("Staff", "Admin")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequest request)
        {
            var result = await _productService.CreateProductAsync(request);
            if (result == null) return BadRequest("Danh mục (CategoryId) không tồn tại. Vui lòng kiểm tra lại!");

            return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, result);
        }
        [HttpPut("{id:guid}")]
        [HasRole("Staff", "Admin")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductRequest request)
        {
            var success = await _productService.UpdateProductAsync(id, request);
            if (!success) return BadRequest("Sản phẩm không tồn tại hoặc Danh mục mới không hợp lệ.");

            return Ok("Cập nhật thông tin sản phẩm thành công.");
        }
        /// <summary>
        /// Chưa Thực Hiện 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("soft-delete/{id:guid}")]
        [HasRole("Staff", "Admin")]
        public async Task<IActionResult> SoftDeleteProduct(Guid id)
        {
            var status = await _productService.SoftDeleteProductAsync(id);

            if (status == -1)
                return NotFound("Không tìm thấy sản phẩm yêu cầu hoặc sản phẩm đã bị xóa trước đó.");

            if (status == -2)
            {
                return BadRequest(new
                {
                    message = "Thao tác bị từ chối! Không thể xóa sản phẩm này vì vẫn còn hàng tồn trong kho (Quantity > 0). Vui lòng thực hiện xuất kho hết trước khi xóa!"
                });
            }

            if (status == -3)
                return StatusCode(500, "Có lỗi xảy ra trong quá trình lưu cập nhật vào hệ sinh thái.");

            return Ok("Xóa sản phẩm thành công (Hệ thống đã chuyển trạng thái sang lưu trữ ẩn).");
        }
    }
}


//Khi viết[Route("api/[controller]")], ký tự trong ngoặc vuông [controller] là một Token (Từ khóa đại diện) quy ước của framework .NET.
//Khi ứng dụng khởi chạy, .NET sẽ tự động thực hiện các bước sau:
//1. Nó tìm đến tên Class: ProductsController.Nó tự động cắt bỏ chữ Controller ở đuôi đi, Còn lại chữ Products.
//2. Nó chuyển chữ này thành viết thường (hoặc giữ nguyên tùy cấu hình cấu trúc, nhưng API không phân biệt chữ hoa chữ thường) -> thu được products.
//3. Nó ráp vào sau chữ api/ mà bạn viết cứng -> Kết quả cuối cùng là: api / products
