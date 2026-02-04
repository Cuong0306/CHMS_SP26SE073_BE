using Microsoft.AspNetCore.Mvc;
using CHMS.BLL.DTOs.Requests.Homestay;  // <--- Namespace Request mới
using CHMS.BLL.DTOs.Responses.Homestay;
using CHMS.BLL.Services.Interfaces;
using CHMS.Domain.Common;
using CHMS.BLL.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;


namespace CHMS.API.Controllers
{
    [Route("api/admin/homestays")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class AdminHomestayController : Controller
    {
        private readonly IHomestayService _homestayService;

        public AdminHomestayController(IHomestayService homestayService)
        {
            _homestayService = homestayService;
        }

        // 1. GET: Lấy danh sách tất cả homestay
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _homestayService.GetAllHomestaysAsync();
            return Ok(ApiResponse<IEnumerable<HomestayResponseDTO>>.SuccessResult(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _homestayService.GetHomestayByIdAsync(id);

            if (result == null)
                return NotFound(ApiResponse<object>.ErrorResult("Không tìm thấy homestay."));

            return Ok(ApiResponse<HomestayResponseDTO>.SuccessResult(result));
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateHomestayRequestDTO request)
        {
            try
            {
                await _homestayService.CreateHomestayAsync(request);
                return Ok(ApiResponse<object>.SuccessResult(null, "Tạo Homestay thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHomestayRequestDTO request)
        {
            try
            {
                await _homestayService.UpdateHomestayAsync(id, request);
                return Ok(ApiResponse<object>.SuccessResult(null, "Cập nhật homestay thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
        {
            try
            {
                await _homestayService.UpdateStatusAsync(id, status);
                return Ok(ApiResponse<object>.SuccessResult(null, "Cập nhật trạng thái thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _homestayService.DeleteHomestayAsync(id);
                return Ok(ApiResponse<object>.SuccessResult(null, "Đã xóa homestay thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }



    }
}
