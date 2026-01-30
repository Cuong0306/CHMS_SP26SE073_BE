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
            var token = GenerateJwtToken(user, roleName);

            return new LoginResponseDTO
            {
                AccessToken = token,
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
    }
    }