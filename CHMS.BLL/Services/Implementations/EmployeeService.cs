using CHMS.BLL.Common.Helpers;
using CHMS.BLL.DTOs.Requests.Employee;
using CHMS.BLL.DTOs.Responses.Employee;
using CHMS.BLL.Services.Interfaces;
using CHMS.DAL.Common;
using CHMS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CHMS.BLL.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Helper map data
        private EmployeeResponseDTO MapToDTO(User user)
        {
            return new EmployeeResponseDTO
            {
                Id = user.Id,
                // Bỏ Username, map thẳng FullName
                FullName = user.FullName,
                Email = user.Email,
                // Entity dùng 'Phone', DTO dùng 'PhoneNumber' (hoặc 'Phone' tùy bạn khai báo bên DTO)
                PhoneNumber = user.Phone,
                Status = user.IsDeleted ? "Banned" : "Active",
                CreatedAt = user.CreatedAt
            };
        }

        // 1. Lấy danh sách nhân viên (Role = "Staff")
        public async Task<IEnumerable<EmployeeResponseDTO>> GetAllEmployeesAsync()
        {
            var staffRole = (await _unitOfWork.Roles.GetAllAsync(r => r.Name == "Staff")).FirstOrDefault();

            if (staffRole == null)
                return new List<EmployeeResponseDTO>();

            var userRoles = await _unitOfWork.UserRoles.GetAllAsync(ur => ur.RoleId == staffRole.Id);
            var staffUserIds = userRoles.Select(ur => ur.UserId).ToList();

            if (!staffUserIds.Any())
                return new List<EmployeeResponseDTO>();

            var staffUsers = await _unitOfWork.Users.GetAllAsync(u => staffUserIds.Contains(u.Id));

            return staffUsers.Select(MapToDTO);
        }

        // 2. Tạo nhân viên mới
        public async Task CreateEmployeeAsync(CreateEmployeeRequestDTO request)
        {
            // a. Check trùng Email (Bỏ check trùng FullName vì người có thể trùng tên)
            var exist = await _unitOfWork.Users.GetAllAsync(u => u.Email == request.Email);
            if (exist.Any())
                throw new Exception("Email đã tồn tại trong hệ thống.");

            // b. Hash Password
            string hashedPassword = PasswordHelper.Hash(request.Password);

            // c. Tạo User
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName, // Dùng FullName từ request
                Email = request.Email,
                Phone = request.PhoneNumber, // Entity là 'Phone'
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                AvatarUrl = request.AvatarUrl // Thêm cái này nếu request có gửi ảnh
            };

            await _unitOfWork.Users.AddAsync(user);

            // d. Gán Role "Staff"
            var staffRole = (await _unitOfWork.Roles.GetAllAsync(r => r.Name == "Staff")).FirstOrDefault();
            if (staffRole != null)
            {
                await _unitOfWork.UserRoles.AddAsync(new UserRole
                {
                    UserId = user.Id,
                    RoleId = staffRole.Id
                });
            }
            else
            {
                throw new Exception("Hệ thống chưa có Role 'Staff'. Vui lòng liên hệ Admin DB.");
            }

            await _unitOfWork.SaveChangesAsync();
        }

        // 3. Lấy chi tiết
        public async Task<EmployeeResponseDTO?> GetEmployeeByIdAsync(Guid id)
        {
            // Bước 1: Lấy User
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return null;

            // Bước 2: (Mới thêm) Check xem User này có phải Staff không?

            // Lấy Role Staff ra trước
            var staffRole = (await _unitOfWork.Roles.GetAllAsync(r => r.Name == "Staff")).FirstOrDefault();
            if (staffRole == null) return null; // Lỗi hệ thống chưa có role

            // Check trong bảng UserRoles xem cặp (UserId, RoleId) có tồn tại không
            var isStaff = (await _unitOfWork.UserRoles
                .GetAllAsync(ur => ur.UserId == user.Id && ur.RoleId == staffRole.Id))
                .Any();

            // Nếu không phải Staff -> coi như không tìm thấy (hoặc throw Exception tùy bạn)
            if (!isStaff) return null;

            // Bước 3: Map data trả về
            return MapToDTO(user);
        }

        // 4. Cập nhật thông tin
        public async Task UpdateEmployeeAsync(Guid id, UpdateEmployeeRequestDTO request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) throw new Exception("Nhân viên không tồn tại.");

            user.FullName = request.FullName; // Update FullName
            user.Phone = request.PhoneNumber; // Update Phone
            user.AvatarUrl = request.AvatarUrl;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        // 5. Xóa nhân viên (Soft Delete)
        public async Task DeleteEmployeeAsync(Guid id)
        {
            await _unitOfWork.Users.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        // 6. Khóa/Mở khóa tài khoản
        public async Task ToggleEmployeeStatusAsync(Guid id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) throw new Exception("Nhân viên không tồn tại.");

            user.IsDeleted = !user.IsDeleted;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public Task AssignHomestayAsync(Guid employeeId, Guid homestayId)
        {
            throw new NotImplementedException("Tính năng này đang tạm khóa.");
        }
    }
}