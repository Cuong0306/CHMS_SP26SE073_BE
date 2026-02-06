using CHMS.BLL.DTOs.Requests.Booking;
using CHMS.BLL.DTOs.Responses.Booking;
using CHMS.BLL.Services.Interfaces;
using CHMS.DAL.Common;
using CHMS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.BLL.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // MAP Entity -> DTO
        private BookingResponseDTO MapToDTO(Booking b)
        {
            return new BookingResponseDTO
            {
                Id = b.Id,
                HomestayId = b.HomestayId,
                HomestayName = b.Homestay?.Name ?? "Loading...",
                CustomerId = b.CustomerId,
                CustomerName = b.Customer?.FullName ?? "Loading...",
                CheckIn = b.CheckIn,
                CheckOut = b.CheckOut,
                TotalNights = b.TotalNights,
                GuestsCount = b.GuestsCount,

                PricePerNight = b.PricePerNightAtBooking,
                SubTotal = b.SubTotal,
                DiscountAmount = b.DiscountAmount,
                TotalPrice = b.TotalPrice,

                Status = b.Status,
                SpecialRequests = b.SpecialRequests,
                ContactPhone = b.ContactPhone,
                CreatedAt = b.CreatedAt
            };
        }

        // 1. Logic tính giá (trả về TotalPrice để khách xem trước)
        public async Task<decimal> CalculatePriceAsync(BookingRequestDTO request)
        {
            if (request.CheckIn >= request.CheckOut)
                throw new Exception("Ngày Check-out phải sau ngày Check-in.");

            var homestay = await _unitOfWork.Homestays.GetByIdAsync(request.HomestayId);
            if (homestay == null) throw new Exception("Homestay không tồn tại.");

            // Tính số đêm (DateOnly trừ nhau ra int ngày luôn)
            int nights = request.CheckOut.DayNumber - request.CheckIn.DayNumber;

            decimal subTotal = homestay.PricePerNight * nights;

            // TODO: Nếu sau này làm Promotion thì trừ tiền ở đây
            decimal discount = 0;

            return subTotal - discount;
        }

        // 2. Tạo Booking
        public async Task<BookingResponseDTO> CreateBookingAsync(Guid customerId, BookingRequestDTO request)
        {
            // a. Validate ngày
            if (request.CheckIn >= request.CheckOut)
                throw new Exception("Ngày Check-out phải sau ngày Check-in.");

            // b. Kiểm tra availability (Trùng lịch)
            // Logic: (StartA < EndB) && (EndA > StartB)
            var conflicts = await _unitOfWork.Bookings.GetAllAsync(b =>
             b.HomestayId == request.HomestayId &&
             b.Status != "CANCELLED" &&
             b.Status != "REJECTED" &&
             request.CheckIn < b.CheckOut &&
             request.CheckOut > b.CheckIn
         );

            if (conflicts.Any())
                throw new Exception("Homestay đã kín lịch trong khoảng thời gian này.");

            // c. Lấy thông tin giá hiện tại
            var homestay = await _unitOfWork.Homestays.GetByIdAsync(request.HomestayId);
            if (homestay == null) throw new Exception("Homestay không tìm thấy.");

            // d. Tính toán các con số
            int nights = request.CheckOut.DayNumber - request.CheckIn.DayNumber;
            decimal pricePerNight = homestay.PricePerNight;
            decimal subTotal = pricePerNight * nights;
            decimal discount = 0; // Tạm thời = 0
            decimal totalPrice = subTotal - discount;

            // e. Tạo Entity (Điền đầy đủ các trường của bạn)
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                HomestayId = request.HomestayId,
                PromotionId = request.PromotionId,

                CheckIn = request.CheckIn,
                CheckOut = request.CheckOut,
                GuestsCount = request.GuestsCount,

                // Snapshot giá
                PricePerNightAtBooking = pricePerNight,
                TotalNights = nights,
                SubTotal = subTotal,
                DiscountAmount = discount,
                TotalPrice = totalPrice,

                SpecialRequests = request.SpecialRequests,
                ContactPhone = request.ContactPhone,
                Status = "PENDING",

                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _unitOfWork.Bookings.AddAsync(booking);

            // Tự động tạo record lịch sử trạng thái (Optional nhưng nên có)
            // var history = new BookingStatusHistory { ... } 

            await _unitOfWork.SaveChangesAsync();

            return MapToDTO(booking);
        }

        // ... Các hàm GetMyBookings, Cancel, GetAll, UpdateStatus giữ nguyên logic
        // chỉ cần sửa lại tên biến cho khớp (VD: SpecialRequest -> SpecialRequests)

        public async Task<IEnumerable<BookingResponseDTO>> GetMyBookingsAsync(Guid customerId)
        {
            var bookings = await _unitOfWork.Bookings.GetAllAsync(b => b.CustomerId == customerId);
            return bookings.Select(MapToDTO);
        }

        public async Task<BookingResponseDTO?> GetBookingByIdAsync(Guid bookingId)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            return booking == null ? null : MapToDTO(booking);
        }

        public async Task CancelBookingAsync(Guid bookingId, Guid customerId)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            if (booking == null) throw new Exception("Booking not found");
            if (booking.CustomerId != customerId) throw new Exception("Unauthorized");
            if (booking.Status != "PENDING") throw new Exception("Chỉ được hủy khi đang chờ duyệt.");

            booking.Status = "CANCELLED";

            // Thêm vào bảng Cancelled nếu cần logic hoàn tiền
            booking.Cancellations.Add(new Cancellation
            {
                Id = Guid.NewGuid(),
                BookingId = bookingId,
                Reason = "Khách tự hủy",
                CreatedAt = DateTime.UtcNow,
                RefundAmount = 0
            });

            _unitOfWork.Bookings.Update(booking);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<BookingResponseDTO>> GetAllBookingsAsync()
        {
            var bookings = await _unitOfWork.Bookings.GetAllAsync();
            return bookings.Select(MapToDTO);
        }

        public async Task UpdateBookingStatusAsync(Guid bookingId, string newStatus)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            if (booking == null) throw new Exception("Not found");

            booking.Status = newStatus;
            booking.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Bookings.Update(booking);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
