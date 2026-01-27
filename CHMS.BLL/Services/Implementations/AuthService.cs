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

namespace CHMS.BLL.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserResponseDTO> RegisterAsync(RegisterRequestDTO dto)
        {
            // 1. Check Email trùng
            // Lưu ý: FindAsync trả về IEnumerable, nên dùng FirstOrDefault() để check
            var existingUsers = await _unitOfWork.Users.GetAsync(u => u.Email == dto.Email);
            var existingUser = await _unitOfWork.Users.GetAsync(u => u.Email == dto.Email);

            if (existingUser != null)
            {
                throw new Exception("Email đã tồn tại!");
            }

            // 2. Map DTO sang Entity User
            var user = _mapper.Map<User>(dto);

            // Tự tạo Guid mới
            user.Id = Guid.NewGuid();
            user.PasswordHash = PasswordHelper.Hash(dto.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.IsDeleted = false;
            user.Status = "Active";

            // 3. Bắt đầu Transaction
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // --- BƯỚC A: LƯU USER ---
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                // --- BƯỚC B: TẠO USER ROLE ---
                // Guid của Role Customer (Lấy từ SQL)
                Guid customerRoleId = Guid.Parse("9D8A6512-B3E0-4235-8679-055270146970");

                var userRole = new UserRole
                {
                    // Nếu bảng UserRoles có cột Id là PK riêng thì cần tạo Guid mới
                    // Id = Guid.NewGuid(), 
                    UserId = user.Id,
                    RoleId = customerRoleId
                };

                // SỬA LỖI Ở ĐÂY: Gọi thông qua property UserRoles đã khai báo trong UnitOfWork
                await _unitOfWork.UserRoles.AddAsync(userRole);

                await _unitOfWork.SaveChangesAsync();

                // Cam kết Transaction
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Lỗi khi tạo tài khoản: " + ex.Message);
            }

            // 4. Trả về kết quả
            var response = _mapper.Map<UserResponseDTO>(user);
            response.RoleName = "Customer";
            return response;
        }
    }
}