using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Warehouse.Application.Middleware;

namespace Warehouse.Presentation.Middleware
{
    public class RoleAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 1. Trích xuất thông tin Endpoint hiện tại đang được Request nhắm tới
            var endpoint = context.GetEndpoint();

            // Nếu Request gọi vào một đường dẫn không tồn tại (Lỗi 404), cho qua để .NET tự xử lý
            if (endpoint == null)
            {
                await _next(context);
                return;
            }

            // 2. Tìm xem Endpoint này (hoặc Controller chứa nó) có cấu hình thẻ [HasRole] không
            var roleAttribute = endpoint.Metadata.GetMetadata<HasRoleAttribute>();

            // Nếu KHÔNG gắn thẻ -> API này là PUBLIC (Ví dụ: Tuyến Đăng ký / Đăng nhập), cho qua luôn
            if (roleAttribute == null)
            {
                await _next(context);
                return;
            }

            // 3. Nếu CÓ gắn thẻ -> Kích hoạt chốt chặn kiểm tra quyền lực
            var user = context.User;

            // CHỐT CHẶN 1: Kiểm tra trạng thái đăng nhập (Lỗi 401)
            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var responseText = JsonSerializer.Serialize(new { message = "Lỗi 401: Bạn chưa đăng nhập hoặc Token hết hạn!" });
                await context.Response.WriteAsync(responseText);
                return; // Ngắt luồng, chặn đứng không cho chạy vào Controller
            }

            // CHỐT CHẶN 2: Kiểm tra phân quyền chi tiết (Lỗi 403)
            var requiredRoles = roleAttribute.AllowedRoles;
            bool hasPermission = false;

            // Quét xem trong danh sách quyền được phép, người dùng hiện tại có sở hữu quyền nào không
            foreach (var role in requiredRoles)
            {
                if (user.IsInRole(role))
                {
                    hasPermission = true;
                    break; // Chỉ cần khớp 1 quyền là hợp lệ
                }
            }

            // Nếu duyệt qua toàn bộ danh sách mà không khớp quyền -> Cấm cửa trả về 403
            if (!hasPermission)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                var responseText = JsonSerializer.Serialize(new { message = "Lỗi 403: Bạn không có quyền truy cập vào chức năng này!" });
                await context.Response.WriteAsync(responseText);
                return; // Ngắt luồng
            }

            // Vượt qua tất cả chốt chặn thành công -> Cho phép thực thi hàm trong Controller
            await _next(context);
        }
    }
}