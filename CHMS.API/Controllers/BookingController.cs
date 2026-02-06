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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingRequestDTO request)
        {
            try
            {
                var userId = GetUserId();
                var result = await _bookingService.CreateBookingAsync(userId, request);
                return Ok(ApiResponse<object>.SuccessResult(result, "Đặt phòng thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetMyBookings()
        {
            try
            {
                var userId = GetUserId();
                var result = await _bookingService.GetMyBookingsAsync(userId);
                return Ok(ApiResponse<object>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            try
            {
                var userId = GetUserId();
                await _bookingService.CancelBookingAsync(id, userId);
                return Ok(ApiResponse<object>.SuccessResult(null, "Đã hủy đơn đặt phòng thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var userId = GetUserId();
            var result = await _bookingService.GetBookingDetailForCustomerAsync(id, userId);

            if (result == null)
                return NotFound(ApiResponse<object>.ErrorResult("Không tìm thấy booking hoặc bạn không có quyền xem."));

            return Ok(ApiResponse<object>.SuccessResult(result));
        }
    }
}
