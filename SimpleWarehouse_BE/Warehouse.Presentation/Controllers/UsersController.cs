using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Warehouse.Application.DTO;
using Warehouse.Application.Interface.IService;
using Warehouse.Application.Middleware;
using Warehouse.Presentation.Middleware;

namespace Warehouse.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // 1. Khởi tạo nhóm tài khoản mẫu
        [HttpPost("seed-samples")]
        [AllowAnonymous]
        public async Task<IActionResult> SeedSamples()
        {
            var result = await _userService.SeedSampleUsersAsync();

            if (!result.Any())
            {
                return BadRequest(new
                {
                    message = "Thao tác không thực hiện! Các tài khoản mẫu (admin, staff) đã được khởi tạo sẵn trên hệ thống DB từ trước."
                });
            }

            return Ok(new
            {
                message = "Khởi tạo dữ liệu tài khoản mẫu thành công! Mật khẩu mặc định là '123'.",
                accounts_created = result
            });
        }

        // 2. API lấy danh sách toàn bộ người dùng hệ thống (Chỉ quyền Admin)
        [HttpGet]
        [HasRole("Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllUsersAsync();
            return Ok(result);
        }

        // 3. đổi role tài khoản
        [HttpPut("{id:guid}/change-role")]
        [HasRole("Admin")]
        public async Task<IActionResult> ChangeRole(Guid id, [FromBody] ChangeRoleRequest request)
        {
            var success = await _userService.ChangeUserRoleAsync(id, request);
            if (!success) return BadRequest("Cập nhật thất bại! Không tìm thấy mã User hoặc tên Role truyền lên không hợp lệ.");

            return Ok($"Đã cập nhật phân quyền tài khoản thành công sang nhóm quyền: {request.NewRole}.");
        }
    }
}