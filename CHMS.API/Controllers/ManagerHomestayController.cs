using CHMS.BLL.DTOs.Requests.Homestay;
using CHMS.BLL.DTOs.Responses.Homestay;
using CHMS.BLL.Services.Interfaces;
using CHMS.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CHMS.API.Controllers
{
    [Route("api/manager/homestays")]
    [ApiController]
    //[Authorize(Roles = "Manager,Homestay Owner")]
    public class ManagerHomestayController : Controller
    {
        private readonly IHomestayService _homestayService;

        public ManagerHomestayController(IHomestayService homestayService)
        {
            _homestayService = homestayService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
                throw new UnauthorizedAccessException("Không tìm thấy thông tin user.");
            return Guid.Parse(userIdString);
        }

        // 1. GET: Lấy danh sách nhà của tui
        [HttpGet]
        public async Task<IActionResult> GetMyHomestays()
        {
            try
            {
                var ownerId = GetCurrentUserId();
                var result = await _homestayService.GetHomestaysByOwnerIdAsync(ownerId);
                return Ok(ApiResponse<IEnumerable<HomestayResponseDTO>>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return Unauthorized(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMyHomestayDetail(Guid id)
        {
            var ownerId = GetCurrentUserId();

            // Check quyền sở hữu
            var isOwner = await _homestayService.IsHomestayOwnerAsync(id, ownerId);
            if (!isOwner)
                return Forbidden(ApiResponse<object>.ErrorResult("Bạn không có quyền xem homestay này."));

            var result = await _homestayService.GetHomestayByIdAsync(id);
            return Ok(ApiResponse<HomestayResponseDTO>.SuccessResult(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMyHomestay(Guid id, [FromBody] UpdateHomestayRequestDTO request)
        {
            try
            {
                var ownerId = GetCurrentUserId();

                // Check quyền sở hữu trước khi cho sửa
                var isOwner = await _homestayService.IsHomestayOwnerAsync(id, ownerId);
                if (!isOwner)
                    return Forbidden(ApiResponse<object>.ErrorResult("Bạn không có quyền sửa homestay này."));

                await _homestayService.UpdateHomestayAsync(id, request);
                return Ok(ApiResponse<object>.SuccessResult(null, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }

        private ObjectResult Forbidden(ApiResponse<object> response)
        {
            return StatusCode(403, response);
        }
    }
}
