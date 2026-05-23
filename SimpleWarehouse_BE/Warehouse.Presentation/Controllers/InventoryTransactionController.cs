using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Warehouse.Application.DTO;
using Warehouse.Application.Interface.IService;
using Warehouse.Application.Middleware;
using Warehouse.Domain.Enum;
using Warehouse.Presentation.Middleware;

namespace Warehouse.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/transactions")]
    public class InventoryTransactionsController : ControllerBase
    {
        private readonly IInventoryTransactionService _transactionService;

        public InventoryTransactionsController(IInventoryTransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // lụm toàn bộ nhật ký giao dịch hệ thống
        [HttpGet]
        [HasRole("Staff", "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _transactionService.GetAllTransactionsAsync();
            return Ok(result);
        }

        // lọc nhật ký kho riêng của từng mặt hàng cụ thể
        [HttpGet("product/{productId:guid}")]
        [HasRole("Staff", "Admin")]
        public async Task<IActionResult> GetByProductId(Guid productId)
        {
            var result = await _transactionService.GetTransactionsByProductIdAsync(productId);
            return Ok(result);
        }

        // thực hiện NHẬP KHO (Tăng hàng tồn)
        [HttpPost("import")]
        [HasRole("Staff", "Manager", "Admin")]
        public async Task<IActionResult> ImportStock([FromBody] TransactionRequest request)
        {
            var userId = GetLoginUserId();
            if (userId == Guid.Empty) return Unauthorized("Phiên xác thực danh tính không hợp lệ hoặc đã hết hạn.");

            var (status, data) = await _transactionService.ProcessTransactionAsync(request, userId, TransactionType.Import);

            if (status == -1)
                return NotFound("Thao tác thất bại! Không tìm thấy mã sản phẩm yêu cầu trong kho.");

            return Ok(new { message = "Giao dịch NHẬP KHO thành công. Số lượng sản phẩm đã tăng.", details = data });
        }

        // thực hiện XUẤT KHO
        [HttpPost("export")]
        [HasRole("Staff", "Admin")]
        public async Task<IActionResult> ExportStock([FromBody] TransactionRequest request)
        {
            var userId = GetLoginUserId();
            if (userId == Guid.Empty) return Unauthorized("Phiên xác thực danh tính không hợp lệ hoặc đã hết hạn.");

            var (status, data) = await _transactionService.ProcessTransactionAsync(request, userId, TransactionType.Export);

            if (status == -1)
                return NotFound("Thao tác thất bại! Không tìm thấy mã sản phẩm yêu cầu trong kho.");

            if (status == -2)
                return BadRequest("Thao tác bị từ chối! Số lượng sản phẩm tồn kho hiện tại không đủ để đáp ứng lệnh xuất này.");

            return Ok(new { message = "Giao dịch XUẤT KHO thành công. Số lượng sản phẩm đã trừ.", details = data });
        }

        // Hàm bóc tách thông tin ID tài khoản từ chuỗi JWT bảo mật
        private Guid GetLoginUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out Guid validUid))
            {
                return validUid;
            }
            return Guid.Empty;
        }
    }
}