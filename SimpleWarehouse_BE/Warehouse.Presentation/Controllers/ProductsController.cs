using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Warehouse.Application.Interface.IService;
using Warehouse.Application.Middleware;
using Warehouse.Domain.Entities;
using Warehouse.Presentation.Controllers;
using Warehouse.Presentation.Middleware;

namespace Warehouse.Presentation.Controllers
{
    [Route("api/[controller]")]
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
    }
}


//Khi viết[Route("api/[controller]")], ký tự trong ngoặc vuông [controller] là một Token (Từ khóa đại diện) quy ước của framework .NET.
//Khi ứng dụng khởi chạy, .NET sẽ tự động thực hiện các bước sau:
//1. Nó tìm đến tên Class: ProductsController.Nó tự động cắt bỏ chữ Controller ở đuôi đi, Còn lại chữ Products.
//2. Nó chuyển chữ này thành viết thường (hoặc giữ nguyên tùy cấu hình cấu trúc, nhưng API không phân biệt chữ hoa chữ thường) -> thu được products.
//3. Nó ráp vào sau chữ api/ mà bạn viết cứng -> Kết quả cuối cùng là: api / products
