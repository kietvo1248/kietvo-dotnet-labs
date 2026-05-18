using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Warehouse.Application.DTO;
using Warehouse.Application.Interface.IService;

namespace Warehouse.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            bool isRegistered = await _authService.RegisterAsync(request);
            if (!isRegistered) return BadRequest("Username đã tồn tại.");
            return Ok("Đăng ký thành công.");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var authResponse = await _authService.LoginAsync(request);
            if (authResponse == null) return Unauthorized("Sai username hoặc password.");
            return Ok(authResponse);
        }
        
    }
}
