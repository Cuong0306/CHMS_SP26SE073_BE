using Microsoft.AspNetCore.Mvc;
using CHMS.Domain.Common;
using CHMS.BLL.DTOs.Requests;
using CHMS.BLL.DTOs.Responses;
using CHMS.BLL.Services.Interfaces;
using CHMS.BLL.DTOs.Requests.Homestay;
using CHMS.BLL.DTOs.Responses.Homestay;

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
            return Ok(ApiResponse<UserResponseDTO>.SuccessResult(result, "Vui lòng kiểm tra email để nhập OTP."));
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDTO request)
        {
            try
            {
                await _authService.VerifyOtpAsync(request.Email, request.OtpCode);
                return Ok(ApiResponse.SuccessResult("Kích hoạt tài khoản thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.ErrorResult(ex.Message));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(ApiResponse<LoginResponseDTO>.SuccessResult(result, "Đăng nhập thành công!"));
            }
            catch (Exception ex)
            {
                // Trả về lỗi 401 (Unauthorized) hoặc 400 tùy bạn
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDTO request)
        {
            try
            {
                var result = await _authService.GoogleLoginAsync(request);
                return Ok(ApiResponse<LoginResponseDTO>.SuccessResult(result, "Đăng nhập Google thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO request)
        {
            try
            {
                await _authService.ForgotPasswordAsync(request.Email);
                return Ok(ApiResponse<object>.SuccessResult(null, "Mã xác nhận đã được gửi vào email của bạn."));
            }
            catch (Exception ex) { return BadRequest(ApiResponse<object>.ErrorResult(ex.Message)); }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO request)
        {
            try
            {
                await _authService.ResetPasswordAsync(request);
                return Ok(ApiResponse<object>.SuccessResult(null, "Đổi mật khẩu thành công. Vui lòng đăng nhập lại."));
            }
            catch (Exception ex) { return BadRequest(ApiResponse<object>.ErrorResult(ex.Message)); }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDTO request)
        {
            try
            {
                // Gọi Service để xử lý xoay vòng token
                var result = await _authService.RefreshTokenAsync(request);

                return Ok(ApiResponse<LoginResponseDTO>.SuccessResult(result, "Làm mới token thành công!"));
            }
            catch (Exception ex)
            {
                // Trả về lỗi 400 nếu token không hợp lệ hoặc hết hạn
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] TokenRequestDTO request)
        {
            try
            {
                // Kiểm tra đầu vào
                if (string.IsNullOrEmpty(request.RefreshToken))
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Refresh Token không được để trống."));
                }

                // Gọi service để hủy token
                await _authService.LogoutAsync(request.RefreshToken);

                return Ok(ApiResponse<object>.SuccessResult(null, "Đăng xuất thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }

    }
}
