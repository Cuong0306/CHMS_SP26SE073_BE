using CHMS.BLL.Services.Interfaces;
using CHMS.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CHMS.API.Controllers
{

    [Route("api/admin/bookings")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class AdminBookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public AdminBookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // 1. GET: Lấy tất cả booking trong hệ thống
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _bookingService.GetAllBookingsAsync();
            return Ok(ApiResponse<object>.SuccessResult(result));
        }
    }
}
