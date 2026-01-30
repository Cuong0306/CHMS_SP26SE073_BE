using CHMS.BLL.Common.Helpers;
using CHMS.BLL.DTOs.Requests;
using CHMS.BLL.DTOs.Responses;
using CHMS.BLL.Services.Interfaces;
using AutoMapper;
using CHMS.DAL.Common;
using CHMS.DAL.Entities; // Cần dòng này để nhận User và UserRole
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;

namespace CHMS.BLL.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache, EmailService emailService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO dto)
        {
            // 1. Tìm user theo Email
            // Lưu ý: Cần Include bảng UserRoles và Role để lấy tên quyền
            // Nếu Repository của bạn chưa hỗ trợ Include, bạn phải query bảng UserRole riêng.
            // Ở đây mình giả sử bạn lấy được user cơ bản trước.
            var user = await _unitOfWork.Users.GetAsync(u => u.Email == dto.Email);

            if (user == null) throw new Exception("Email hoặc mật khẩu không đúng.");

            // 2. Check mật khẩu (Dùng hàm Verify hash)
            bool isPasswordValid = PasswordHelper.Verify(dto.Password, user.PasswordHash);
            if (!isPasswordValid) throw new Exception("Email hoặc mật khẩu không đúng.");

            // 3. Check trạng thái Active (Quan trọng vì có vụ OTP)
            if (user.Status != "Active") throw new Exception("Tài khoản chưa được kích hoạt. Vui lòng xác thực OTP.");
            if (user.IsDeleted) throw new Exception("Tài khoản đã bị khóa.");

            // 4. Lấy Role của User (Vì bảng UserRoles tách riêng)
            // Query bảng UserRole để tìm RoleId, rồi tìm RoleName
            // (Đoạn này tùy vào cách bạn viết Repository, dưới đây là cách thủ công an toàn nhất)
            var userRole = await _unitOfWork.UserRoles.GetAsync(ur => ur.UserId == user.Id);
            var roleName = "Customer"; // Mặc định
            if (userRole != null)
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(userRole.RoleId);
                if (role != null) roleName = role.Name;
            }

            // 5. Tạo JWT Token
            var accessToken = GenerateJwtToken(user, roleName);

            // 2. Tạo Refresh Token Entity
            var refreshTokenStr = GenerateRandomString(35); // Hàm random chuỗi

            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = refreshTokenStr,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // Hết hạn sau 7 ngày
                RevokedAt = null,                       // Chưa bị thu hồi
                ReplacedByToken = null,                 // Chưa bị thay thế
                DeviceInfo = "Unknown"                  // Tạm thời để Unknown, sau này bạn lấy từ Header User-Agent
            };

            // 3. Lưu vào DB
            await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            return new LoginResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenStr,
                Email = user.Email,
                FullName = user.FullName,
                Role = roleName
            };
        }


        private string GenerateJwtToken(User user, string roleName)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Role, roleName), // <--- Quan trọng để phân quyền
        new Claim("FullName", user.FullName)
    };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["DurationInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public async Task<UserResponseDTO> RegisterAsync(RegisterRequestDTO dto)
        {
            // 1. Check Email trùng trong DB (Vẫn phải check để tránh lỗi)
            var existingUser = await _unitOfWork.Users.GetAsync(u => u.Email == dto.Email);
            if (existingUser != null) throw new Exception("Email đã tồn tại!");

            // 2. Sinh OTP
            var otpCode = new Random().Next(100000, 999999).ToString();

            // 3. Tạo object tạm để lưu vào RAM (Gồm thông tin đăng ký + Mã OTP)
            var tempRegistration = new
            {
                UserData = dto, // Lưu lại thông tin họ nhập (Họ tên, pass...)
                Otp = otpCode
            };

            // 4. Lưu vào RAM (Hết hạn sau 5 phút)
            // Key bây giờ là "TEMP_USER_email"
            _cache.Set($"TEMP_USER_{dto.Email}", tempRegistration, TimeSpan.FromMinutes(5));

            // 5. Gửi Email
            await _emailService.SendEmailAsync(dto.Email, "Xác thực tài khoản",
                $"<h3>Mã OTP: <b style='color:red'>{otpCode}</b></h3>");

            // 6. Trả về kết quả ảo (Vì chưa có ID user thật)
            return new UserResponseDTO
            {
                Email = dto.Email,
                FullName = dto.FullName,
            };
        }

        public async Task<bool> VerifyOtpAsync(string email, string inputOtp)
        {
            // 1. Lấy cục thông tin trong RAM ra
            string cacheKey = $"TEMP_USER_{email}";

            // Dùng dynamic hoặc tạo class riêng để hứng dữ liệu
            if (!_cache.TryGetValue(cacheKey, out object? cachedData))
            {
                throw new Exception("Mã OTP đã hết hạn hoặc bạn chưa đăng ký.");
            }

            // Ép kiểu dữ liệu lấy từ Cache (dùng dynamic cho nhanh gọn, hoặc tạo class DTO)
            // Lưu ý: Cần reference Microsoft.CSharp nếu dùng dynamic
            dynamic data = cachedData!;
            string storedOtp = data.Otp;
            RegisterRequestDTO userDto = data.UserData;

            // 2. Check OTP
            if (storedOtp != inputOtp) throw new Exception("Mã OTP không chính xác.");

            // ==========================================================
            // GIỜ MỚI BẮT ĐẦU LƯU VÀO DB (CODE CHUYỂN TỪ HÀM REGISTER SANG)
            // ==========================================================

            var user = _mapper.Map<User>(userDto);
            user.Id = Guid.NewGuid();
            user.PasswordHash = PasswordHelper.Hash(userDto.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.Status = "Active"; // Active luôn vì đã check OTP rồi
            user.IsDeleted = false;

            // Tìm Role
            var customerRole = await _unitOfWork.Roles.GetAsync(r => r.Name == "Customer");

            // Gán Role
            user.UserRoles.Add(new UserRole
            {
                RoleId = customerRole.Id,
                AssignedAt = DateTime.UtcNow
            });

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Lỗi lưu DB: " + ex.Message);
            }

            // 3. Xóa Cache cho sạch
            _cache.Remove(cacheKey);

            return true;
        }

        public async Task<LoginResponseDTO> GoogleLoginAsync(GoogleLoginRequestDTO dto)
        {
            // 1. Xác thực Token với Google (Bước quan trọng nhất)
            GoogleJsonWebSignature.Payload payload;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _configuration["Authentication:Google:ClientId"] } // Check đúng ClientId của mình
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, settings);
            }
            catch
            {
                throw new Exception("Token Google không hợp lệ hoặc đã hết hạn.");
            }

            // 2. Kiểm tra xem Email này đã có trong DB chưa
            var user = await _unitOfWork.Users.GetAsync(u => u.Email == payload.Email);
            var roleName = "Customer";

            if (user == null)
            {
                // === TRƯỜNG HỢP 1: CHƯA CÓ -> TỰ ĐỘNG ĐĂNG KÝ ===
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = payload.Email,
                    FullName = payload.Name, // Lấy tên từ Google

                    // Vì login Google nên không có pass, ta set random hoặc chuỗi rỗng
                    // Lưu ý: Logic Login thường phải check nếu PasswordHash null thì chặn login bằng pass
                    PasswordHash = "GOOGLE_AUTH_NO_PASSWORD",

                    Status = "Active", // Google đã xác thực rồi nên Active luôn
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                // Tìm Role Customer
                var customerRole = await _unitOfWork.Roles.GetAsync(r => r.Name == "Customer");

                // Thêm UserRoles
                user.UserRoles.Add(new UserRole
                {
                    RoleId = customerRole.Id,
                    AssignedAt = DateTime.UtcNow
                });

                // Lưu vào DB
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                // === TRƯỜNG HỢP 2: ĐÃ CÓ -> CHECK TRẠNG THÁI ===
                if (user.IsDeleted) throw new Exception("Tài khoản đã bị khóa.");

                // Lấy Role hiện tại để tạo Token
                var userRole = await _unitOfWork.UserRoles.GetAsync(ur => ur.UserId == user.Id);
                if (userRole != null)
                {
                    var role = await _unitOfWork.Roles.GetByIdAsync(userRole.RoleId);
                    if (role != null) roleName = role.Name;
                }
            }

            // 3. Tạo JWT Token (Dùng lại hàm cũ của bạn)
            var accessToken = GenerateJwtToken(user, roleName);

            return new LoginResponseDTO
            {
                AccessToken = accessToken,
                Email = user.Email,
                FullName = user.FullName,
                Role = roleName
            };
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _unitOfWork.Users.GetAsync(u => u.Email == email);
            if (user == null) throw new Exception("Email không tồn tại trong hệ thống.");

            // Sinh mã OTP 6 số
            var otpCode = new Random().Next(100000, 999999).ToString();

            // Lưu vào RAM (Key khác với key đăng ký nhé, đặt là RESET_OTP_)
            // Hết hạn sau 15 phút
            _cache.Set($"RESET_OTP_{email}", otpCode, TimeSpan.FromMinutes(15));

            // Gửi mail
            await _emailService.SendEmailAsync(email, "Yêu cầu đặt lại mật khẩu",
                $"<h3>Mã xác nhận của bạn là: <b style='color:red'>{otpCode}</b></h3><p>Mã này có hiệu lực 15 phút.</p>");

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDTO dto)
        {
            // Check OTP trong RAM
            if (!_cache.TryGetValue($"RESET_OTP_{dto.Email}", out string? storedOtp))
            {
                throw new Exception("Mã OTP đã hết hạn hoặc không đúng.");
            }

            if (storedOtp != dto.OtpCode) throw new Exception("Mã OTP không chính xác.");

            // Lấy user ra để đổi pass
            var user = await _unitOfWork.Users.GetAsync(u => u.Email == dto.Email);
            if (user == null) throw new Exception("Lỗi hệ thống: User không tìm thấy.");

            // Hash mật khẩu mới
            user.PasswordHash = PasswordHelper.Hash(dto.NewPassword);

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync(); // Nếu cần

            // Xóa OTP cho sạch
            _cache.Remove($"RESET_OTP_{dto.Email}");

            return true;
        }

       
        public async Task<LoginResponseDTO> RefreshTokenAsync(TokenRequestDTO dto)
        {
            // 1. Tìm token trong DB
            var storedToken = await _unitOfWork.RefreshTokens.GetAsync(x => x.Token == dto.RefreshToken);

            if (storedToken == null)
            {
                throw new Exception("Refresh Token không tồn tại.");
            }

            // 2. Kiểm tra tính hợp lệ
            // A. Đã bị thu hồi chưa?
            if (storedToken.RevokedAt != null)
            {
                // Kịch bản bảo mật: Nếu token đã bị thu hồi mà vẫn cố dùng -> Có thể token bị trộm
                // Ở đây mình chặn lại. (Nâng cao: Có thể thu hồi luôn tất cả token của user này để an toàn)
                throw new Exception("Token này đã bị thu hồi (Revoked).");
            }

            // B. Đã hết hạn chưa?
            if (storedToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new Exception("Token đã hết hạn. Vui lòng đăng nhập lại.");
            }

            // C. Đã được sử dụng để đổi cái mới chưa? (ReplacedByToken)
            if (!string.IsNullOrEmpty(storedToken.ReplacedByToken))
            {
                throw new Exception("Token này đã được sử dụng. Vui lòng đăng nhập lại.");
            }

            // 3. XỬ LÝ XOAY VÒNG (Token Rotation)
            var user = await _unitOfWork.Users.GetAsync(u => u.Id == storedToken.UserId);
            if (user == null) throw new Exception("User không tồn tại.");

            // Tạo Access Token mới
            // Lưu ý: Lấy lại role cũ hoặc query lại DB để lấy role mới nhất
            var userRole = await _unitOfWork.UserRoles.GetAsync(ur => ur.UserId == user.Id);
            var roleName = "Customer"; // Logic lấy tên role của bạn
            if (userRole != null)
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(userRole.RoleId);
                roleName = role?.Name ?? "Customer";
            }

            var newAccessToken = GenerateJwtToken(user, roleName);
            var newRefreshTokenStr = GenerateRandomString(35);

            // 4. Cập nhật Token CŨ (Đánh dấu là đã dùng và bị thay thế)
            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.ReplacedByToken = newRefreshTokenStr;
            _unitOfWork.RefreshTokens.Update(storedToken);

            // 5. Tạo Token MỚI
            var newRefreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = newRefreshTokenStr,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                RevokedAt = null,
                ReplacedByToken = null,
                DeviceInfo = storedToken.DeviceInfo // Giữ nguyên thông tin thiết bị cũ
            };

            await _unitOfWork.RefreshTokens.AddAsync(newRefreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            return new LoginResponseDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenStr,
                Email = user.Email,
                FullName = user.FullName,
                Role = roleName
            };
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            // 1. Tìm Refresh Token trong DB
            var storedToken = await _unitOfWork.RefreshTokens.GetAsync(x => x.Token == refreshToken);

            // Nếu không tìm thấy (hoặc đã bị xóa rồi) thì coi như thành công luôn
            if (storedToken == null) return true;

            // 2. Đánh dấu là đã thu hồi (Revoke)
            // Lưu ý: Không cần xóa khỏi DB để còn lưu vết lịch sử đăng nhập
            storedToken.RevokedAt = DateTime.UtcNow;

            // 3. Cập nhật
            _unitOfWork.RefreshTokens.Update(storedToken);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
    }