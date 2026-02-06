using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.BLL.DTOs.Responses.Booking
{
    public class BookingResponseDTO
    {
        public Guid Id { get; set; }
        public Guid HomestayId { get; set; }
        public string HomestayName { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public int TotalNights { get; set; } // Thêm

        public int GuestsCount { get; set; }

        // Phần tiền chi tiết
        public decimal PricePerNight { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalPrice { get; set; }

        public string Status { get; set; } = string.Empty;
        public string? SpecialRequests { get; set; }
        public string? ContactPhone { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
