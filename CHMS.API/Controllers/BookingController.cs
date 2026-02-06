using CHMS.BLL.DTOs.Requests.Booking;
using CHMS.BLL.Services.Interfaces;
using CHMS.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CHMS.API.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // Hàm phụ: Lấy ID user từ Token
        private Guid GetUserId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        // 1. POST: Tính toán giá tiền trước khi đặt
        [HttpPost("calculate")]
        public async Task<IActionResult> Calculate([FromBody] BookingRequestDTO request)
        {
            try
            {
                var price = await _bookingService.CalculatePriceAsync(request);
                return Ok(ApiResponse<decimal>.SuccessResult(price));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }
    }
}
