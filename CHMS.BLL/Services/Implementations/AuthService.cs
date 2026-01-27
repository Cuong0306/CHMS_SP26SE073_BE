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
            var existingUser = await _unitOfWork.Users.GetAsync(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                throw new Exception("Email đã tồn tại!");
            }

            // 2. Map DTO sang User
            var user = _mapper.Map<User>(dto);

            // Config các thông tin User
            user.Id = Guid.NewGuid();
            user.PasswordHash = PasswordHelper.Hash(dto.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.Status = "Active";
            user.IsDeleted = false;

            // 3. Tìm Role Customer
            // Lưu ý: Dùng AsNoTracking() nếu có thể để tối ưu, nhưng GetAsync thường đã xử lý rồi
            var customerRole = await _unitOfWork.Roles.GetAsync(r => r.Name == "Customer");
            if (customerRole == null) throw new Exception("Lỗi: Không tìm thấy quyền Customer");

            // =========================================================================
            // 🔥 KHẮC PHỤC LỖI TẠI ĐÂY: DÙNG NAVIGATION COLLECTION
            // Thay vì lưu User trước -> Rồi mới tạo UserRole -> Rồi lưu UserRole
            // Ta nhét thẳng UserRole vào trong danh sách của User luôn.
            // Entity Framework sẽ tự lo việc: Lưu User -> Lấy ID User -> Gán vào UserRole -> Lưu UserRole
            // =========================================================================

            var userRole = new UserRole
            {
                // Không cần gán UserId thủ công nữa, EF tự điền
                RoleId = customerRole.Id,
                AssignedAt = DateTime.UtcNow // Nhớ dòng này kẻo lỗi DateTime
            };

            // Thêm vào danh sách UserRoles của chính user đó
            user.UserRoles.Add(userRole);

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // 4. CHỈ CẦN LƯU USER LÀ ĐỦ (Nó sẽ tự lưu cả Role đi kèm)
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                // Lấy lỗi gốc để dễ debug
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception("Lỗi Database: " + msg);
            }

            // 5. Trả kết quả
            var response = _mapper.Map<UserResponseDTO>(user);
            response.RoleName = "Customer";
            return response;
        }
    }
    }