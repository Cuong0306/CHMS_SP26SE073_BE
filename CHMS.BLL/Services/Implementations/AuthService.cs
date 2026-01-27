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

namespace CHMS.BLL.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly EmailService _emailService;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache, EmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
            _emailService = emailService;
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