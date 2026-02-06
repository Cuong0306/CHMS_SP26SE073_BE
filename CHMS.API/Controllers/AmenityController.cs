using CHMS.BLL.DTOs.Requests.Amenity;
using CHMS.BLL.DTOs.Responses.Amenity;
using CHMS.BLL.Services.Interfaces;
using CHMS.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CHMS.API.Controllers
{
    [ApiController]
    public class AmenityController : Controller
    {
        private readonly IAmenityService _amenityService;

        public AmenityController(IAmenityService amenityService)
        {
            _amenityService = amenityService;
        }

        // 1. GET: Public List
        [HttpGet("api/amenities")]
        [AllowAnonymous] // Public ai cũng xem được
        public async Task<IActionResult> GetAll()
        {
            var result = await _amenityService.GetAllAmenitiesAsync();
            return Ok(ApiResponse<IEnumerable<AmenityResponseDTO>>.SuccessResult(result));
        }

        [HttpPost("api/admin/amenities")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] AmenityRequestDTO request)
        {
            try
            {
                await _amenityService.CreateAmenityAsync(request);
                return Ok(ApiResponse<object>.SuccessResult(null, "Tạo tiện nghi thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }

        [HttpPut("api/admin/amenities/{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AmenityRequestDTO request)
        {
            try
            {
                await _amenityService.UpdateAmenityAsync(id, request);
                return Ok(ApiResponse<object>.SuccessResult(null, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }
    }
}
