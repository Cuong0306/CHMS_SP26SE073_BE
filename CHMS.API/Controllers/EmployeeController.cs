using CHMS.BLL.DTOs.Requests.Employee;
using CHMS.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CHMS.API.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var result = await _employeeService.GetAllEmployeesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(Guid id)
        {
            try
            {
                var result = await _employeeService.GetEmployeeByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { message = "Không tìm thấy nhân viên." });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _employeeService.CreateEmployeeAsync(request);
                return Ok(new { message = "Tạo nhân viên thành công." });
            }
            catch (Exception ex)
            {
                // Trả về lỗi 400 kèm message (ví dụ: Email trùng)
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _employeeService.UpdateEmployeeAsync(id, request);
                return Ok(new { message = "Cập nhật thông tin thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            try
            {
                await _employeeService.DeleteEmployeeAsync(id);
                return Ok(new { message = "Đã xóa nhân viên (Soft Delete)." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
