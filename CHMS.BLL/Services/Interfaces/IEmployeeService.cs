using CHMS.BLL.DTOs.Requests.Employee;
using CHMS.BLL.DTOs.Responses.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.BLL.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeResponseDTO>> GetAllEmployeesAsync();
        Task<EmployeeResponseDTO?> GetEmployeeByIdAsync(Guid id);
        Task CreateEmployeeAsync(CreateEmployeeRequestDTO request);
        Task UpdateEmployeeAsync(Guid id, UpdateEmployeeRequestDTO request);
        Task DeleteEmployeeAsync(Guid id);
        Task ToggleEmployeeStatusAsync(Guid id); // Khóa/Mở khóa
        Task AssignHomestayAsync(Guid employeeId, Guid homestayId); // Phân công
    }
}
