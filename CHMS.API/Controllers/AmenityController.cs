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
    }
}
