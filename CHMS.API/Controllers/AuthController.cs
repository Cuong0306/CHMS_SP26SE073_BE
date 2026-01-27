using Microsoft.AspNetCore.Mvc;
using CHMS.Domain.Common;
using CHMS.BLL.DTOs.Requests;
using CHMS.BLL.DTOs.Responses;
using CHMS.BLL.Services.Interfaces;

namespace CHMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Inject Service vào Controller
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // API: POST api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            // Gọi Service để xử lý logic
            var result = await _authService.RegisterAsync(request);

            // Trả về kết quả chuẩn format ApiResponse
            // (Hàm SuccessResult bạn đã có trong file ApiResponse.cs ở Domain)
            return Ok(ApiResponse<UserResponseDTO>.SuccessResult(result, "Đăng ký thành công!"));
        }
    }
}
