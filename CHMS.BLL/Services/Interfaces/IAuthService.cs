using CHMS.BLL.DTOs.Requests;
using CHMS.BLL.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.BLL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponseDTO> RegisterAsync(RegisterRequestDTO registerDto);

        Task<bool> VerifyOtpAsync(string email, string inputOtp);

        public Task<LoginResponseDTO> LoginAsync(LoginRequestDTO dto);

        public Task<LoginResponseDTO> GoogleLoginAsync(GoogleLoginRequestDTO dto);

        public Task<bool> ForgotPasswordAsync(string email);

        public Task<bool> ResetPasswordAsync(ResetPasswordRequestDTO dto);
    }
}
