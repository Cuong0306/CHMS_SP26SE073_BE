using CHMS.BLL.DTOs.Requests.Booking;
using CHMS.BLL.DTOs.Responses.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.BLL.Services.Interfaces
{
    public interface IBookingService
    {
        public Task<decimal> CalculatePriceAsync(BookingRequestDTO request);

        public Task<BookingResponseDTO> CreateBookingAsync(Guid customerId, BookingRequestDTO request);


        public Task<IEnumerable<BookingResponseDTO>> GetMyBookingsAsync(Guid customerId);

        public Task<BookingResponseDTO?> GetBookingByIdAsync(Guid bookingId);


        public Task CancelBookingAsync(Guid bookingId, Guid customerId);

        public Task<IEnumerable<BookingResponseDTO>> GetAllBookingsAsync();


        public Task UpdateBookingStatusAsync(Guid bookingId, string newStatus);

        Task<BookingResponseDTO?> GetBookingDetailForCustomerAsync(Guid bookingId, Guid customerId);
        Task ModifyBookingAsync(Guid bookingId, Guid customerId, BookingRequestDTO request);
        Task<string> GetCancellationPolicyAsync(Guid bookingId);
        Task AddSpecialRequestAsync(Guid bookingId, Guid customerId, string specialRequest);
    }
}
